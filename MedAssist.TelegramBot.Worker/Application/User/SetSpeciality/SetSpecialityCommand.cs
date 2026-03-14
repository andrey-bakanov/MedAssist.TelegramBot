using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.User.SetSpeciality;

public sealed class SetSpecialityCommand : BotCommandBase
{
    public SetSpecialityCommand([NotNull] Update message, IEnumerable<string> arguments) : base(message, arguments)
    {
    }

    public override string Name => BotCommandNames.SetSpecialityCommandName;
}