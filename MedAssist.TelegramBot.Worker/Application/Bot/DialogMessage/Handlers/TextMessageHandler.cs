using MedAssist.TelegramBot.Worker.Extensions;
using Telegram.Bot.Types.Enums;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;

public class TextMessageHandler : IMessageContentHandler
{
    public MessageType SupportedType => MessageType.Text;

    public Task<string?> ProcessAsync(DialogMessageCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(command.Text);
    }
}