using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.Unknown;

public sealed class UnknownCommand : BotCommandBase
{
    public UnknownCommand([NotNull] Update message) : base(message)
    {
        RegistrationRequired = false;
    }

    public override string Name => string.Empty;
}