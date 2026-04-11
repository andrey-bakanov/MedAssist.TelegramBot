namespace MedAssist.TelegramBot.Worker.Services.Api;

public class ChatMessageDto
{
    public string? Answer { get; set; }

    public Guid? ConversationId { get; set; }

    public Guid RequestId { get; set; }
}
