using MedAssist.TelegramBot.Worker.Extensions;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Services.Media;

public class MediaProcessingService : IMediaProcessingService
{
    public async Task<(MemoryStream FileStream, string FileName)> DownloadFileAsync(ITelegramBotClient client, string fileId, long? fileSize)
    {
        var (stream, fileName, _) = await DownloadFileWithMimeTypeAsync(client, fileId, fileSize);
        return (stream, fileName);
    }

    public async Task<(MemoryStream FileStream, string FileName, string MimeType)> DownloadFileWithMimeTypeAsync(ITelegramBotClient client, string fileId, long? fileSize)
    {
        var telegramFile = await client.GetFile(fileId);
        int streamSize = fileSize.HasValue ? (int)fileSize.Value : (int)(telegramFile.FileSize ?? 1024 * 1024);
        MemoryStream stream = new MemoryStream(streamSize);
        await client.DownloadFile(telegramFile.FilePath, stream);
        string fileName = telegramFile.FilePath ?? fileId;
        string mimeType = GetMimeTypeFromFilePath(telegramFile.FilePath);
        return (stream, fileName, mimeType);
    }

    private static string GetMimeTypeFromFilePath(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return "application/octet-stream";

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}