using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.User.Register;

public sealed class RegisterCommand : BotCommandBase
{
    public RegisterCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {
        RegistrationRequired = false;
    }

    public override string Name => BotCommandNames.RegisterCommandName;
}