using MedAssist.TelegramBot.Worker.Services.Api;

namespace MedAssist.TelegramBot.Worker.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMedAssistAuthApiClient _client;

    public class Z { }

    public AuthenticationService(IMedAssistAuthApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<AuthDto?> GetToken(string apiKey)
    {
        var request = new AuthRequest
        {
            Payload = new Z(),
            Type = "api_key",
        };

        string apiKeyHeaderValue = $"ApiKey {apiKey}";

        var response = await _client.GetToken(apiKeyHeaderValue, request);

        await response.EnsureSuccessfulAsync();

        return response.Content;
    }
}