using MedAssist.TelegramBot.Worker.Exceptions;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services.Api;
using Microsoft.Extensions.Caching.Memory;

namespace MedAssist.TelegramBot.Worker.Services;

public class DataService : IDataService
{
    private readonly IMedAssistApiClient _client;
    private readonly IMemoryCache _memoryCache;

    public DataService(IMedAssistApiClient client, IMemoryCache memoryCache)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<IEnumerable<Speciality>> GetSpecialitiesAsync()
    {
        string cacheKey = "specilities";

        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (entry) => {
            var response = await _client.GetSpecialities();
            if(response.IsSuccessStatusCode)
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(5);
            }
            else
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow;
            }

            var data = response.Content;

            return data;
        });

        return result ?? Enumerable.Empty<Speciality>();
    }

    public async ValueTask<string> GetConfirmationTextAsync()
    {
        string cacheKey = "static_license";

        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (entry) => {
            var response = await _client.GetStaticContent(StaticContentCodes.Licencse);

            var data = string.Empty;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(5);

                data = response.Content.Value.Replace("\\n", Environment.NewLine);
            }
            else
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow;
            }

            return data;
        });

        return result ?? "Условия использования и лицензирования.";
    }

    public async ValueTask<string> GetHelpTextAsync()
    {
        string cacheKey = "static_help";

        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (entry) => {
            var response = await _client.GetStaticContent(StaticContentCodes.Help);

            var data = string.Empty;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(5);

                data = response.Content.Value.Replace("\\n", Environment.NewLine);
            }
            else
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow;
            }

            

            return data;
        });

        return result ?? "Бот помощник помогает сформировать данные для помощи пациентам.";
    }

    public Task UpdateSpecialityAsync(long userId, string specialityCode)
    {
        var request = new SpecialityUpdateRequest 
        { 
            Code = specialityCode 
        };

        return _client.UpdateSpeciality(userId, request);
    }

    public Task RegisterAsync(long userId, string username)
    {
        RegistrationRequest registrationRequest = new RegistrationRequest
        {
            Nickname = username,
            Confirmed = true,
            TelegramUserId = userId
        };

        return _client.Register(registrationRequest);
    }

    public Task UnregisterAsync(long userId)
    {
        return _client.Unregister(userId);
    }

    public async Task<UserProfile?> GetUserInfoAsync(long userId)
    {
        string cacheKey = "profile_"+ userId;

        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (entry) => {
            var response = await _client.GetProfile(userId);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(5);
                }
                else
                {
                    entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(30);
                }
            }
            else
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow;
            }

            var data = response.Content;

            return data;
        });

        return result;
    }

    public async Task<IEnumerable<Client>> GetClientsAsync(long userId)
    {
        string cacheKey = "clients_" + userId;

        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async (entry) => {
            var clients = await _client.GetClients(userId);

            entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5);

            return clients;
        });

        return result ?? Enumerable.Empty<Client>();
    }

    public async Task<Client> GetClientInfoAsync(long userId, Guid clientId)
    {
        return await _client.GetClient(userId, clientId);
    }

    public async Task DeleteClientInfoAsync(long userId, Guid clientId)
    {
        await _client.DeleteClient(userId, clientId);

        string cacheKey = "clients_" + userId;
        _memoryCache.Remove(cacheKey);
    }

    public async Task CreateClientInfoAsync(long userId, string clientName)
    {
        var client = new Client
        { 
            Nickname = clientName 
        };

        await _client.CreateClient(userId, client);

        string cacheKey = "clients_" + userId;
        _memoryCache.Remove(cacheKey);
    }

    public async Task SetupClientSessionAsync(long userId, Guid clientId)
    {
        var request = new SetClientSessionRequest
        {
            PatientId = clientId
        };

        await _client.SetupClientSession(userId, request);

        string cacheKey = "profile_" + userId;
        _memoryCache.Remove(cacheKey);
    }

    public async Task ClearClientSessionAsync(long userId)
    {
        await _client.ClearClientSession(userId);

        string cacheKey = "profile_" + userId;
        _memoryCache.Remove(cacheKey);
    }

    public async Task<ChatMessageDto> SendChatMessage(long userId, string text, Guid requestId, Guid? conversationId = null)
    {
        ChatMessageRequest request = new ChatMessageRequest()
        {
            Text = text,
            RequestId = requestId,
            ConversationId = conversationId
        };

        var response = await _client.SendMessage(userId, request);

        if(response.IsSuccessStatusCode)
        {
            return response.Content!;
        }

        if(response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
        {
            throw new DialogDenideException(ResourceMain.PaymentRequired);
        }

        throw new DialogDenideException();
    }
}