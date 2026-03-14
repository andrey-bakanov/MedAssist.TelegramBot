using Refit;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public interface IMedAssistAuthApiClient
{
    [Post("/v1/auth/token")]
    Task<Refit.ApiResponse<AuthDto>> GetToken([Header("Authorization")] string apiKey, [Body] AuthRequest request);
}