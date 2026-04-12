using MedAssist.TelegramBot.Worker.Services.Media;
using MedAssist.TelegramBot.Worker.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;

public class PhotoMessageHandler : IMessageContentHandler
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly ICommonOcrService _ocrService;
    private readonly IMediaProcessingService _mediaProcessingService;
    private readonly ILogger<PhotoMessageHandler> _logger;

    public MessageType SupportedType => MessageType.Photo;

    public PhotoMessageHandler(
        ITelegramBotClient telegramClient,
        ICommonOcrService ocrService,
        IMediaProcessingService mediaProcessingService,
        ILogger<PhotoMessageHandler> logger)
    {
        _telegramClient = telegramClient;
        _ocrService = ocrService;
        _mediaProcessingService = mediaProcessingService;
        _logger = logger;
    }

    public async Task<string?> ProcessAsync(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        await _telegramClient.SendChatAction(command.ChatId, ChatAction.Typing);

        var photo = command.Message!.Photo?.LastOrDefault();
        if (photo == null)
        {
            _logger.LogWarning("No photo found in Photo message");
            return null;
        }

        var (fileStream, fileName) = await _mediaProcessingService.DownloadFileAsync(_telegramClient, photo.FileId, photo.FileSize);

        using (fileStream)
        {
            fileStream.Position = 0;
            var result = await _ocrService.ConvertImageToTextAsync(fileStream, fileName, "image/jpeg");

            if (result.Success)
            {
                return result.Text;
            }

            _logger.LogError("Image OCR failed: {Error}", result.Error);
            return null;
        }
    }
}