namespace MedAssist.TelegramBot.Worker.Models;

public record StaticContentItem
{
    public required string Code { get; init; }
    public required string Value { get; init; }
}