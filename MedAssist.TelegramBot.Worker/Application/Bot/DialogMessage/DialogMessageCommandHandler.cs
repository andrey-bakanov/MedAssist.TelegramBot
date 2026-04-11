using System.Text.RegularExpressions;
using MedAssist.TelegramBot.Worker.Extensions;
using MedAssist.TelegramBot.Worker.Helpers;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Refit;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;

public class DialogMessageCommandHandler : ICommandHandler<DialogMessageCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;
    private readonly IAsrApiClient _asrApiClient;
    private readonly ILogger<DialogMessageCommandHandler> _logger;

    private const int ChatTypingActionIntervalMilliseconds = 7_000;

    public DialogMessageCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService,
        IAsrApiClient asrApiClient,
        ILogger<DialogMessageCommandHandler> logger)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
        _asrApiClient = asrApiClient;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        string? textMessage = command.AutoAnswer ? command.Text : command.Message.Text;

        if (command.Message?.Type == MessageType.Voice)
        {
            textMessage = await Transcribe(command);
        }

        return await HandleTextMessage(textMessage, command, cancellationToken);
    }

    private async Task<Unit> HandleTextMessage(string textMessage, DialogMessageCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(textMessage))
        {
            return Unit.Value;
        }

        // Check for registration
        UserState? userState = _userStateService.GetState(command.UserId);

        //Проверить на специализацию
        if(textMessage.StartsWith("subspec_"))
        {
            string subSpecname = textMessage.Replace("subspec_", string.Empty);
            string llmResponse = userState.LastLLMResponse;
        }

        //Send message to LLM
        var responseTask = userState.ClientName != null ?
           _dataService.SendClientChatMessage(command.UserId, textMessage, Guid.Parse(userState.ClientName.Id)) :
           _dataService.SendChatMessage(command.UserId, textMessage);

        while (!responseTask.IsCompleted) {
            //Show to user a typing animation
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

        List<InlineKeyboardButton> subspecButtons = new();

        MatchCollection matches = Regex.Matches(message, @"<subspec>(.*?)</subspec>");
        foreach (Match match in matches.Take(3))
        {
            string subspec = match.Groups[1].Value;
            string subspecData = $"subspec_{match.Groups[1].Value}";
            subspecButtons.Add(InlineKeyboardButton.WithCallbackData(subspec, subspecData));
        }
        message = Regex.Replace(message, @"<subspec>.*?</subspec>", string.Empty, RegexOptions.IgnoreCase).EscapeMarkdownSpecialCharacters();

        _logger.LogInformation(message);

        IReadOnlyList<string> messageParts = MessageSplitter.Split(message);
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

    private async Task<string?> Transcribe(DialogMessageCommand command)
    {
        //Show to user a typing animation
        await _telegramClient.SendChatAction(
                chatId: command.ChatId,
                action: ChatAction.Typing
            );

        var message = command.Message;

        var voice = message.Voice;
        string fileId = voice.FileId;

        var telegramFile = await _telegramClient.GetFile(fileId);

        using MemoryStream voiceStream = new MemoryStream((int)telegramFile.FileSize!);

        await _telegramClient.DownloadFile(telegramFile.FilePath, voiceStream);
        
        voiceStream.Position = 0;
        StreamPart part = new StreamPart(voiceStream, "voice.oga", voice.MimeType);
        var response = await _asrApiClient.TranscribeAudio(part);

        if(response.IsSuccessful)
        {
            return response.Content.Text;
        }

        _logger.LogError(response.Error.Message);
        return null;
    }
}