namespace MedAssist.TelegramBot.Worker.Services.Api;

public class ChatMessageRequest
{
    public required string Text { get; set; }

    public required Guid RequestId { get; set; }

    public string? SpecialtyCodeOverride { get; set; }
}
