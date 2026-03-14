namespace MedAssist.TelegramBot.Worker.Services.Api;

public class AuthDto
{
    /// <summary>
    /// Токен
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Время устаревания
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Тип токена
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// Тип пользователя
    /// </summary>
    public string ActorType { get; set; }
}
