namespace MedAssist.TelegramBot.Worker.Services.Api;

public class RegistrationRequest
{
    public required long TelegramUserId { get; set; }

    public required string Nickname { get; set; }

    public bool Confirmed { get; set; }
}