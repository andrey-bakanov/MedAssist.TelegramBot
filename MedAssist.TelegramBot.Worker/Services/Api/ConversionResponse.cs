namespace MedAssist.TelegramBot.Worker.Services.Api;

public class ConversionResponse
{
    public bool Success { get; set; }
    public string? Markdown { get; set; }
    public string? Error { get; set; }
}