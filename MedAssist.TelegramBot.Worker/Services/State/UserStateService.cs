using MedAssist.TelegramBot.Worker.Models;
using System.Collections.Concurrent;

namespace MedAssist.TelegramBot.Worker.Services.State;

public class UserStateService
{
    private ConcurrentDictionary<long, UserState> States { get; set; } = new ConcurrentDictionary<long, UserState>();

    public UserState GetState(long userId)
    {
        bool stateFound =  States.TryGetValue(userId, out UserState? state);
        if(!stateFound)
        {
            throw new Exception($"{userId} user state not found");
        }

        return state!;
    }

    public UserState UpdateState(long userId, UserState state)
    {
        return States.AddOrUpdate(userId, state, (userId, stateOld) => state);
    }

    public UserState UpdateRegistrationState(long userId, bool isRegistered)
    {
        UserState oldState = GetState(userId);

        var newState = new UserState()
        {
            Identity = oldState.Identity,
            IsRegistered = isRegistered,
            LastCommandName = oldState?.LastCommandName,
            AwaitingReplyMessageId = oldState?.AwaitingReplyMessageId,
            ClientName = oldState?.ClientName
        };

        return States.AddOrUpdate(userId, newState, (userId, stateOld) => newState);
    }

    public UserState UpdateLastCommand(long userId, string lastCommandName)
    {
        UserState? state = GetState(userId);

        state.LastCommandName = lastCommandName;

        return state;
    }

    public UserState UpdateClientSession(long userId, NamedItem? clientid)
    {
        UserState? state = GetState(userId);

        state.ClientName = clientid;

        return state;
    }

    public UserState UpdateReplyMessageId(long userId, int? messageId)
    {
        UserState? state = GetState(userId);

        state.AwaitingReplyMessageId = messageId;

        return state;
    }

    public UserState UpdateLastLlmResponseSession(long userId, string response)
    {
        UserState? state = GetState(userId);

        state.LastLLMResponse = response;

        return state;
    }

    public UserState OverrideSpeciality(long userId, string? speciality)
    {
        UserState? state = GetState(userId);

        state.OverridedSpeciality = speciality;

        return state;
    }

    public async ValueTask<UserState> EnsureState(long userId, long chatId, Func<long,Task<UserState>> stateFactory)
    {
        bool stateFound = States.TryGetValue(userId, out UserState? state);
        if (stateFound)
        {
            if (state!.Identity.ChatId == chatId)
            {
                return state!;
            }
        }

        var newState = await stateFactory(userId);

        return States.AddOrUpdate(userId, newState, (userId, stateOld) => newState);
    }
}