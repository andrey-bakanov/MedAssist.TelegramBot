using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.StopDialog;

public sealed class StopDialogCommand : BotCommandBase
{
    public StopDialogCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.StopDialogCommandName;
}