using MedAssist.TelegramBot.Worker.Services.Media;
using MedAssist.TelegramBot.Worker.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;

public class DocumentMessageHandler : IMessageContentHandler
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly ICommonOcrService _ocrService;
    private readonly IMediaProcessingService _mediaProcessingService;
    private readonly ILogger<DocumentMessageHandler> _logger;

    public MessageType SupportedType => MessageType.Document;

    public DocumentMessageHandler(
        ITelegramBotClient telegramClient,
        ICommonOcrService ocrService,
        IMediaProcessingService mediaProcessingService,
        ILogger<DocumentMessageHandler> logger)
    {
        _telegramClient = telegramClient;
        _ocrService = ocrService;
        _mediaProcessingService = mediaProcessingService;
        _logger = logger;
    }

    public async Task<string?> ProcessAsync(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        await _telegramClient.SendChatAction(command.ChatId, ChatAction.Typing);

        var document = command.Message!.Document!;
        var (fileStream, fileName, mimeType) = await _mediaProcessingService.DownloadFileWithMimeTypeAsync(_telegramClient, document.FileId, document.FileSize);

        using (fileStream)
        {
            fileStream.Position = 0;
            var result = mimeType switch
            {
                "application/pdf" => await _ocrService.ConvertPdfToMarkdownAsync(fileStream, fileName, mimeType),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => await _ocrService.ConvertDocxToMarkdownAsync(fileStream, fileName, mimeType),
                _ => null
            };

            if (result?.Success == true)
            {
                return result.Text;
            }

            _logger.LogError("OCR failed: {Error}", result?.Error);
            return null;
        }
    }
}