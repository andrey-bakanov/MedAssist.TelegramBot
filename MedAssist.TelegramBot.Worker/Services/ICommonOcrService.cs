using MedAssist.TelegramBot.Worker.Services.Api;

namespace MedAssist.TelegramBot.Worker.Services;

public interface ICommonOcrService
{
    Task<OcrConversionResult> ConvertImageToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertImageToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertImageToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default);

    Task<OcrConversionResult> ConvertPdfToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertPdfToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertPdfToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default);

    Task<OcrConversionResult> ConvertDocxToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertDocxToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertDocxToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default);

    Task<OcrConversionResult> ConvertImageToTextAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertImageToTextAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<OcrConversionResult> ConvertImageToTextAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default);
}

public class OcrConversionResult
{
    public bool Success { get; set; }
    public string? Text { get; set; }
    public string? Error { get; set; }

    public static OcrConversionResult FromResponse(ConversionResponse response)
    {
        return new OcrConversionResult
        {
            Success = response.Success,
            Text = response.Markdown,
            Error = response.Error
        };
    }
}