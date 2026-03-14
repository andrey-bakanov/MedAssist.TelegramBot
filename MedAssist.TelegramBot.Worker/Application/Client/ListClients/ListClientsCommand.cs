using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.ListClients;

public sealed class ListClientsCommand : BotCommandBase
{
    public ListClientsCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.ClientsCommandName;
}