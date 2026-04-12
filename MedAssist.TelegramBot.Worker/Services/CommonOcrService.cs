using MedAssist.TelegramBot.Worker.Services.Api;
using Refit;

namespace MedAssist.TelegramBot.Worker.Services;

public class CommonOcrService : ICommonOcrService
{
    private readonly ICommonOcrApiClient _client;

    public CommonOcrService(ICommonOcrApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public Task<OcrConversionResult> ConvertImageToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertImageToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToMarkdown(CreateStreamPart(fileStream, fileName, contentType)));

    public Task<OcrConversionResult> ConvertImageToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertPdfToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertPdfToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertPdfToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertPdfToMarkdown(CreateStreamPart(fileStream, fileName, contentType)));

    public Task<OcrConversionResult> ConvertPdfToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertPdfToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertDocxToMarkdownAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertDocxToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertDocxToMarkdownAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertDocxToMarkdown(CreateStreamPart(fileStream, fileName, contentType)));

    public Task<OcrConversionResult> ConvertDocxToMarkdownAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertDocxToMarkdown(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertImageToTextAsync(byte[] fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToText(CreateStreamPart(fileData, fileName, contentType)));

    public Task<OcrConversionResult> ConvertImageToTextAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToText(CreateStreamPart(fileStream, fileName, contentType)));

    public Task<OcrConversionResult> ConvertImageToTextAsync(Memory<byte> fileData, string fileName, string contentType, CancellationToken cancellationToken = default)
        => ConvertAsync(() => _client.ConvertImageToText(CreateStreamPart(fileData, fileName, contentType)));

    private async Task<OcrConversionResult> ConvertAsync(Func<Task<ApiResponse<Api.ConversionResponse>>> apiCall)
    {
        var response = await apiCall();

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return OcrConversionResult.FromResponse(response.Content);
        }

        return new OcrConversionResult
        {
            Success = false,
            Error = response.Error?.Message ?? "Unknown error occurred"
        };
    }

    private static StreamPart CreateStreamPart(byte[] data, string fileName, string contentType)
    {
        return new StreamPart(new MemoryStream(data), fileName, contentType);
    }

    private static StreamPart CreateStreamPart(Stream stream, string fileName, string contentType)
    {
        return new StreamPart(stream, fileName, contentType);
    }

    private static StreamPart CreateStreamPart(Memory<byte> data, string fileName, string contentType)
    {
        return new StreamPart(new MemoryStream(data.ToArray()), fileName, contentType);
    }
}