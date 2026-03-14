using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.Help;

public sealed class HelpCommand : BotCommandBase
{
    public HelpCommand([NotNull] Update message) : base(message)
    {
        RegistrationRequired = false;
    }

    public override string Name => BotCommandNames.HelpCommandName;
}