using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;
using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Processing;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;

public class DialogMessageCommandHandler : ICommandHandler<DialogMessageCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;
    private readonly IEnumerable<IMessageContentHandler> _contentHandlers;
    private readonly LlmResponseProcessor _llmResponseProcessor;
    private readonly ILogger<DialogMessageCommandHandler> _logger;

    private const int ChatTypingActionIntervalMilliseconds = 7_000;

    public DialogMessageCommandHandler(
        ITelegramBotClient telegramClient,
        UserStateService userStateService,
        IDataService dataService,
        IEnumerable<IMessageContentHandler> contentHandlers,
        LlmResponseProcessor llmResponseProcessor,
        ILogger<DialogMessageCommandHandler> logger)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
        _contentHandlers = contentHandlers;
        _llmResponseProcessor = llmResponseProcessor;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        string? textMessage = command.AutoAnswer ? command.Text : command.Message?.Text;

        if (command.Message?.Type != null)
        {
            var handler = _contentHandlers.FirstOrDefault(h => h.SupportedType == command.Message.Type);
            if (handler != null)
            {
                textMessage = await handler.ProcessAsync(command, cancellationToken);
            }

            if (command.Message.Caption != null && textMessage != null)
            {
                textMessage = $"#{command.Message.Caption}" + Environment.NewLine + textMessage;
            }
        }

        return await HandleTextMessage(textMessage, command, cancellationToken);
    }

    private async Task<Unit> HandleTextMessage(string? textMessage, DialogMessageCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(textMessage))
        {
            return Unit.Value;
        }

        UserState? userState = _userStateService.GetState(command.UserId);

        string? subSpecCode = userState.OverridedSpeciality;

        //button with subspec clicked
        if (textMessage.StartsWith("subspec_"))
        {
            subSpecCode = textMessage.Replace("subspec_", string.Empty);
            var specialities = await _dataService.GetSpecialitiesAsync();
            var subspec = specialities.FirstOrDefault(x => x.Code == subSpecCode);
            if (subspec != null)
            {
                textMessage = userState.LastLLMResponse;

                _userStateService.OverrideSpeciality(command.UserId, subSpecCode);

                await _telegramClient.SendMessage(
                    command.ChatId,
                    $"Уточнение со специализацией {subspec.Title}",
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken);
            }
            else
            {
                _logger.LogWarning($"Requested subspec {subSpecCode}  - reset subspec.");

                subSpecCode = null;
                _userStateService.OverrideSpeciality(command.UserId, null);

                await _telegramClient.SendMessage(
                    command.ChatId,
                    $"Возвращена основная специализация",
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken);
            }            
        }

        var responseTask = userState.ClientName != null
           ? _dataService.SendClientChatMessage(command.UserId, textMessage, Guid.Parse(userState.ClientName.Id), subSpecCode)
           : _dataService.SendChatMessage(command.UserId, textMessage, subSpecCode);

        while (!responseTask.IsCompleted)
        {
            await _telegramClient.SendChatAction(
                    chatId: command.ChatId,
                    action: ChatAction.Typing,
                    cancellationToken: cancellationToken
                );

            Task delayTask = Task.Delay(ChatTypingActionIntervalMilliseconds, cancellationToken);

            await Task.WhenAny(responseTask, delayTask);
        }

        var response = await responseTask;
        _userStateService.UpdateLastLlmResponseSession(command.UserId, response.Answer ?? string.Empty);

        string message = (response.Answer ?? ResourceMain.WaitingForReply);

        var (cleanMessage, subspecButtons) = await _llmResponseProcessor.ProcessAsync(message);

        _logger.LogInformation(cleanMessage);

        IReadOnlyList<string> messageParts = _llmResponseProcessor.SplitMessage(cleanMessage);
        bool isLastPart = false;
        for (int i = 0; i < messageParts.Count; i++)
        {
            isLastPart = (i == messageParts.Count - 1);
            InlineKeyboardMarkup? replyMarkup = null;

            if (isLastPart)
            {
                if (!string.IsNullOrEmpty(subSpecCode))
                {
                    //Работаем в рамках переопределенной специализации
                    replyMarkup = new InlineKeyboardMarkup(
                            new InlineKeyboardButton("Вернуться к своей специализации", "subspec_reset")
                        );
                }
                else if (subspecButtons.Count > 0)
                {
                    //Рисуем кнопки в обычном режиме
                    replyMarkup = new InlineKeyboardMarkup(subspecButtons.Chunk(2).Select(chunk => chunk.ToArray()));
                }

            }

            await _telegramClient.SendMessage(
                command.ChatId,
                messageParts[i],
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}