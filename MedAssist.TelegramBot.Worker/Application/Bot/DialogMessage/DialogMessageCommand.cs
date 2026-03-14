using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;

public sealed class DialogMessageCommand : BotCommandBase
{
    public DialogMessageCommand([NotNull] Update message) : base(message)
    {

    }

    public override string Name => string.Empty;
}