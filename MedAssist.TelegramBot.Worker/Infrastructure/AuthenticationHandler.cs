using MedAssist.TelegramBot.Worker.Configuration;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MedAssist.TelegramBot.Worker.Infrastructure;

/// <summary>
/// Обработчик HTTP-запросов для автоматического обновления токена при ошибке 401.
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly DataServiceConfiguration _dataServiceConfiguration;
    private readonly TokenStorage _tokenStorage;
        /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler" /> class.
    /// </summary>
    /// <param name="authService">Сервис аутентификации для получения токена.</param>
    /// <param name="logger">Логгер для записи сообщений.</param>
    /// <param name="dataServiceConfiguration">Конфигурация сервиса данных, содержащая API-ключ.</param>
    public AuthenticationHandler(IAuthenticationService authService, ILogger<AuthenticationHandler> logger, IOptions<DataServiceConfiguration> dataServiceConfiguration, TokenStorage tokenStorage)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dataServiceConfiguration = dataServiceConfiguration?.Value ?? throw new ArgumentNullException(nameof(dataServiceConfiguration));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        // Проверка существующего токена
        if (await _tokenStorage.IsValidAsync()) {
            var currentToken = await _tokenStorage.GetTokenAsync();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", currentToken);

            response = await base.SendAsync(request, cancellationToken);
        }

        // Проверка на ошибку 401 (Unauthorized)
        if (response == null || response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Получен ответ 401 Unauthorized. Попытка обновить токен...");

            // Получение нового токена
            var authDto = await _authService.GetToken(_dataServiceConfiguration.ApiKey);
            string? token = authDto?.AccessToken;
            if (token != null)
            {
                // Сохранение нового токена с временем истечения
                var expiration = DateTimeOffset.UtcNow.AddSeconds(authDto.ExpiresIn).DateTime;
                await _tokenStorage.SetTokenAsync(authDto.AccessToken, expiration);
            }
            else
            {
                _logger.LogError("Не удалось получить новый токен. Операция завершена.");
                return response;
            }

            // Обновление заголовка Authorization
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Токен обновлен. Повторная отправка запроса...");

            // Повторная отправка запроса с новым токеном
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}