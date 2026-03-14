using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.DeleteClient;

public sealed class DeleteClientCommand : BotCommandBase
{
    public DeleteClientCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {
    }

    public override string Name => BotCommandNames.DeleteClientCommandName;
}