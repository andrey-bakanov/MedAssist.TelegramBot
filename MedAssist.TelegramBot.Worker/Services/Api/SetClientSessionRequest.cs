namespace MedAssist.TelegramBot.Worker.Services.Api;

public sealed class SetClientSessionRequest
{
    public Guid PatientId { get; set; }
}