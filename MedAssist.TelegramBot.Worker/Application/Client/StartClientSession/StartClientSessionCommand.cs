using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.StartClientSession;

public sealed class StartClientSessionCommand : BotCommandBase
{
    public StartClientSessionCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.StartClientSessionCommandName;
}