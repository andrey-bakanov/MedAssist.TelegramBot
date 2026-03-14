using MedAssist.TelegramBot.Worker.Services.Api;

namespace MedAssist.TelegramBot.Worker.Services;

/// <summary>
/// Интерфейс для сервиса аутентификации.
/// </summary>
public interface IAuthenticationService
{
    /// <inheritdoc />
    Task<AuthDto?> GetToken(string apikey);
}