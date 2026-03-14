using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.User.Me;

public sealed class MeCommand : BotCommandBase
{
    public MeCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.MeCommandName;
}