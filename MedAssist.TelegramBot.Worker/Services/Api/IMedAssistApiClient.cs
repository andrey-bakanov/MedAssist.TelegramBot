using MedAssist.TelegramBot.Worker.Models;
using Refit;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public interface IMedAssistApiClient
{
    [Get("/v1/reference/specializations")]
    Task<Refit.ApiResponse<IEnumerable<Speciality>>> GetSpecialities();

    [Patch("/v1/me/specialization")]
    Task UpdateSpeciality([Header("X-Telegram-User-Id")] long userId, [Body] SpecialityUpdateRequest request);

    [Get("/v1/static-content/{code}")]
    Task<Refit.ApiResponse<StaticContentItem>> GetStaticContent(string code);

    [Post("/v1/registration")]
    Task Register([Body] RegistrationRequest request);

    [Delete("/v1/registration")]
    Task<Refit.ApiResponse<string>> Unregister([Header("X-Telegram-User-Id")] long userId);

    [Get("/v1/me")]
    Task<Refit.ApiResponse<UserProfile>> GetProfile([Header("X-Telegram-User-Id")] long userId);

    [Get("/v1/patients")]
    Task<IEnumerable<Client>> GetClients([Header("X-Telegram-User-Id")] long userId);

    [Post("/v1/patients")]
    Task CreateClient([Header("X-Telegram-User-Id")] long userId, [Body] Client client);

    [Get("/v1/patients/{clientId}")]
    Task<Client> GetClient([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Delete("/v1/patients/{clientId}")]
    Task<Refit.ApiResponse<Client>> DeleteClient([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Put("/v1/me/active-patient")]
    Task SetupClientSession([Header("X-Telegram-User-Id")] long userId, [Body] SetClientSessionRequest request);

    [Delete("/v1/me/active-patient")]
    Task ClearClientSession([Header("X-Telegram-User-Id")] long userId);

    [Post("/v1/bot/chat/ask")]
    Task<Refit.ApiResponse<ChatMessageDto>> SendMessage([Header("X-Telegram-User-Id")] long userId, [Body] ChatMessageRequest request);
}