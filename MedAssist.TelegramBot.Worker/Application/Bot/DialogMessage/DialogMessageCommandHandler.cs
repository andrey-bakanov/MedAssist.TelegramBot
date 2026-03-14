using MedAssist.TelegramBot.Worker.Extensions;
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

    private const int ChatTypingActionIntervalMilliseconds = 5_000;

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
        string? textMessage = command.Message.Text;

        if (command.Message.Type == MessageType.Voice)
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
        Guid? conversationId = userState.ConversationId;

        //Send message to LLM
        Guid requestId = Guid.NewGuid();
        var responseTask = _dataService.SendChatMessage(command.UserId, textMessage, requestId, conversationId);
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

        userState = _userStateService.UpdateConversationIdState(command.UserId, response.ConversationId);

        ReplyKeyboardMarkup? keyboardMarkup = null;
        if (conversationId == null || conversationId != response.ConversationId)
        {
            KeyboardButton[]? keyboardButtons = null;
            if (userState?.ClientName != null)
            {
                keyboardButtons = [new KeyboardButton($"{BotCommandNames.StopDialogCommandName} [{userState.ClientName.Name}]")];
            }
            else
            {
                keyboardButtons = [new KeyboardButton($"{BotCommandNames.StartNewDialogCommandName}")];
            }

            keyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true,
                IsPersistent = true
            };
        }

        await _telegramClient.SendMessage(
            command.ChatId,
            (response.Answer ?? ResourceMain.WaitingForReply).EscapeMarkdownSpecialCharacters(),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: keyboardMarkup,
            cancellationToken: cancellationToken);

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