namespace MedAssist.TelegramBot.Worker.Models;

public record UserChatIdentity
{
    public required long UserId { get; init; }
    public required long ChatId { get; init; }
}