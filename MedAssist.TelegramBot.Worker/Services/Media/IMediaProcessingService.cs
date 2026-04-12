using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Services.Media;

public interface IMediaProcessingService
{
    Task<(MemoryStream FileStream, string FileName)> DownloadFileAsync(ITelegramBotClient client, string fileId, long? fileSize);
    Task<(MemoryStream FileStream, string FileName, string MimeType)> DownloadFileWithMimeTypeAsync(ITelegramBotClient client, string fileId, long? fileSize);
}