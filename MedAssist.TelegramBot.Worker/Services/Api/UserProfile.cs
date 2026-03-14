namespace MedAssist.TelegramBot.Worker.Services.Api;

public class UserProfile
{
    public Guid DoctorId { get; set; }
    public required long TelegramUserId { get; set; }

    public required string Nickname { get; set; }

    public Guid? LastSelectedPatientId { get; set; }

    public string? LastSelectedPatientNickname { get; set; }

    public IEnumerable<Speciality>? Specializations { get; set; }

    public long TokenBalance { get; set; }
}