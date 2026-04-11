namespace MedAssist.TelegramBot.Worker.Services.Api;

public class PatientDialogDto
{
    public Guid PatientId { get; set; }
    public string? Nickname { get; set; }
    public DateTime LastConversationAt { get; set; }

}
