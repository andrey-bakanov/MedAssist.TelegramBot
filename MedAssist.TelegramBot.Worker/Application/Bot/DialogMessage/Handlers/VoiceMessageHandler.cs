using MedAssist.TelegramBot.Worker.Services.Media;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Extensions;
using Refit;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;

public class VoiceMessageHandler : IMessageContentHandler
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IAsrApiClient _asrApiClient;
    private readonly IMediaProcessingService _mediaProcessingService;
    private readonly ILogger<VoiceMessageHandler> _logger;

    public MessageType SupportedType => MessageType.Voice;

    public VoiceMessageHandler(
        ITelegramBotClient telegramClient,
        IAsrApiClient asrApiClient,
        IMediaProcessingService mediaProcessingService,
        ILogger<VoiceMessageHandler> logger)
    {
        _telegramClient = telegramClient;
        _asrApiClient = asrApiClient;
        _mediaProcessingService = mediaProcessingService;
        _logger = logger;
    }

    public async Task<string?> ProcessAsync(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        await _telegramClient.SendChatAction(command.ChatId, ChatAction.Typing);

        var voice = command.Message!.Voice!;
        var (fileStream, fileName) = await _mediaProcessingService.DownloadFileAsync(_telegramClient, voice.FileId, voice.FileSize);

        using (fileStream)
        {
            fileStream.Position = 0;
            StreamPart part = new StreamPart(fileStream, fileName, voice.MimeType);
            var response = await _asrApiClient.TranscribeAudio(part);

            if (response.IsSuccessful)
            {
                return response.Content?.Text;
            }

            _logger.LogError(response.Error?.Message);
            return null;
        }
    }
}