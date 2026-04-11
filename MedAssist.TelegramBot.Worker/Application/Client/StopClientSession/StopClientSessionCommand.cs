using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.StopClientSession;

public sealed class StopClientSessionCommand : BotCommandBase
{
    public StopClientSessionCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.StopClientSessionCommandName;
}