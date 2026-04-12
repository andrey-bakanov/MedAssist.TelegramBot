using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;
using Telegram.Bot.Types.Enums;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;

public interface IMessageContentHandler
{
    MessageType SupportedType { get; }
    Task<string?> ProcessAsync(DialogMessageCommand command, CancellationToken cancellationToken);
}