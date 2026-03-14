using MedAssist.TelegramBot.Worker.Application;
using MedAssist.TelegramBot.Worker.Exceptions;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Infrastructure.Pipelines;

public sealed class RegistrationValidatorPipeline<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : BotCommandBase
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;

    public RegistrationValidatorPipeline(ITelegramBotClient telegramClient, UserStateService userStateService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        UserState currentState = _userStateService.GetState(message.UserId);
        if (message.RegistrationRequired && !currentState.IsRegistered)
        {
            throw new DialogDenideException();
        }

        var response = await next(message, cancellationToken);
        return response;
    }
}