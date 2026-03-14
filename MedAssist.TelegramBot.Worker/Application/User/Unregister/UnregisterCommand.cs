using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.User.Unregister;

public sealed class UnregisterCommand : BotCommandBase
{
    public UnregisterCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {
    }

    public override string Name => BotCommandNames.UnregisterCommandName;
}