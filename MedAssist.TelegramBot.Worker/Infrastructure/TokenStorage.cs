using System.Threading;

namespace MedAssist.TelegramBot.Worker.Infrastructure;

/// <summary>
/// Потокобезопасное хранилище токена аутентификации.
/// Обеспечивает наличие только одного активного токена для всего приложения.
/// </summary>
public class TokenStorage
{
    private string? _token;
    private DateTime _expirationTime;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Проверяет, действителен ли текущий токен и не истек ли срок его действия.
    /// </summary>
    /// <returns>True, если токен действителен и не истек; иначе false.</returns>
    public async Task<bool> IsValidAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return !string.IsNullOrEmpty(_token) && DateTime.UtcNow < _expirationTime;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Получает текущий действительный токен.
    /// </summary>
    /// <returns>Текущий токен или null, если он недействителен.</returns>
    public async Task<string?> GetTokenAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return (DateTime.UtcNow < _expirationTime) ? _token : null;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Устанавливает новый токен и его время истечения.
    /// </summary>
    /// <param name="token">Новый токен.</param>
    /// <param name="expirationTime">Время истечения токена.</param>
    public async Task SetTokenAsync(string token, DateTime expirationTime)
    {
        await _semaphore.WaitAsync();
        try
        {
            _token = token;
            _expirationTime = expirationTime;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Очищает текущий токен.
    /// </summary>
    public async Task ClearTokenAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _token = null;
            _expirationTime = DateTime.MinValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}