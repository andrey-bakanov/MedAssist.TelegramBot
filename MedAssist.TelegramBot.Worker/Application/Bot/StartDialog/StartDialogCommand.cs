using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.StartDialog;

public sealed class StartDialogCommand : BotCommandBase
{
    public StartDialogCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.StartNewDialogCommandName;
}