namespace MedAssist.TelegramBot.Worker.Services.Api;

public sealed class Client
{
    public Guid Id { get; set; }

    public required string Nickname { get; set; }

    public string? Allergies { get; set; }

    public string? ChronicConditions { get; set; }
}