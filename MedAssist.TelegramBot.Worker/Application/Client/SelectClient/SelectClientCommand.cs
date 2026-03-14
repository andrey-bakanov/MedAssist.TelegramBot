using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Client.SelectClient;

public sealed class SelectClientCommand : BotCommandBase
{
    public SelectClientCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {

    }

    public override string Name => BotCommandNames.SelectClientInfoCommandName;
}