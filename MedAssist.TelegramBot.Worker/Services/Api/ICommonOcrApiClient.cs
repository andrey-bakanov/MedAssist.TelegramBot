using Refit;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public interface ICommonOcrApiClient
{
    [Multipart]
    [Post("/convert/image-markdown")]
    Task<ApiResponse<ConversionResponse>> ConvertImageToMarkdown([AliasAs("file")] StreamPart file);

    [Multipart]
    [Post("/convert/pdf-markdown")]
    Task<ApiResponse<ConversionResponse>> ConvertPdfToMarkdown([AliasAs("file")] StreamPart file);

    [Multipart]
    [Post("/convert/docx-markdown")]
    Task<ApiResponse<ConversionResponse>> ConvertDocxToMarkdown([AliasAs("file")] StreamPart file);

    [Multipart]
    [Post("/convert/image-text")]
    Task<ApiResponse<ConversionResponse>> ConvertImageToText([AliasAs("file")] StreamPart file);

    [Get("/")]
    Task<ApiResponse<object>> GetRoot();
}