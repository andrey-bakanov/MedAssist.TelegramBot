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

    [Get("/v1/me/chat/last-patient")]
    Task<PatientDialogDto> GetLastDialogPatient([Header("X-Telegram-User-Id")] long userId);

    [Get("/v1/patients")]
    Task<IEnumerable<Client>> GetClients([Header("X-Telegram-User-Id")] long userId);

    [Post("/v1/patients")]
    Task CreateClient([Header("X-Telegram-User-Id")] long userId, [Body] Client client);

    [Get("/v1/patients/{clientId}")]
    Task<Client> GetClient([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Delete("/v1/patients/{clientId}")]
    Task<Refit.ApiResponse<Client>> DeleteClient([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Post("/v1/bot/chat/general/ask")]
    Task<ApiResponse<ChatMessageDto>> SendGeneralMessage([Header("X-Telegram-User-Id")] long userId, [Body] ChatMessageRequest request);

    [Post("/v1/patients/{clientId}/chat/current/complete")]
    Task CompleteDialog([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Post("/v1/patients/{clientId}/chat/current/ask")]
    Task<ApiResponse<ChatMessageDto>> SendClientDialogMessage([Header("X-Telegram-User-Id")] long userId, Guid clientId, [Body] ChatMessageRequest request);

    [Post("/v1/patients/{clientId}/chat/conversations")]
    Task<StartNewDialogDto> StartClientDialog([Header("X-Telegram-User-Id")] long userId, Guid clientId);

    [Post("/v1/patients/{clientId}/chat/current/complete")]
    Task CompleteClientDialog([Header("X-Telegram-User-Id")] long userId, Guid clientId);
}