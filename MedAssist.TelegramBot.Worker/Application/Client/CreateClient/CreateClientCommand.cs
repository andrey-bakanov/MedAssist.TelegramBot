using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.CreateClient;

public sealed class CreateClientCommand : BotCommandBase
{
    public CreateClientCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.CreateClientCommandName;
}