namespace MedAssist.TelegramBot.Worker.Services.Api;

public class AuthRequest
{
    /// <summary>
    /// Тип клиента
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Данные аутентификации
    /// </summary>
    public required object Payload { get; set; }
}
