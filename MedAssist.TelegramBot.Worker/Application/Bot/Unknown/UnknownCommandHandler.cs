using MedAssist.TelegramBot.Worker.Application.Bot.Unknown;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Application.Bot.Unknown;

public class UnknownCommandHandler : ICommandHandler<UnknownCommand>
{
    private readonly ITelegramBotClient _telegramClient;

    public UnknownCommandHandler(ITelegramBotClient telegramClient, UserStateService userStateService)
    {
        _telegramClient = telegramClient;
    }

    public async ValueTask<Unit> Handle(UnknownCommand command, CancellationToken cancellationToken)
    {
        return Unit.Value;
    }
}