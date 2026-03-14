using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.Start;

public sealed class StartCommand : BotCommandBase
{
    public StartCommand([NotNull] Update message) : base(message)
    {
        RegistrationRequired = false;
    }

    public override string Name => BotCommandNames.StartCommandName;
}