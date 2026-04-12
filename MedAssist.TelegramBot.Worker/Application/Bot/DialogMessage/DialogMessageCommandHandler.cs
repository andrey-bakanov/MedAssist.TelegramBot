using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;
using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Processing;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using MedAssist.TelegramBot.Worker.Extensions;
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

        if (textMessage.StartsWith("subspec_"))
        {
            string subSpecname = textMessage.Replace("subspec_", string.Empty);
            string llmResponse = userState.LastLLMResponse;
        }

        var responseTask = userState.ClientName != null
           ? _dataService.SendClientChatMessage(command.UserId, textMessage, Guid.Parse(userState.ClientName.Id))
           : _dataService.SendChatMessage(command.UserId, textMessage);

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

        var (cleanMessage, subspecButtons) = _llmResponseProcessor.Process(message);

        _logger.LogInformation(cleanMessage);

        IReadOnlyList<string> messageParts = _llmResponseProcessor.SplitMessage(cleanMessage);
        bool isLastPart = false;
        for (int i = 0; i < messageParts.Count; i++)
        {
            isLastPart = (i == messageParts.Count - 1);
            InlineKeyboardMarkup? replyMarkup = (isLastPart && subspecButtons.Count > 0)
                ? new InlineKeyboardMarkup(subspecButtons.Chunk(2).Select(chunk => chunk.ToArray()))
                : null;

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