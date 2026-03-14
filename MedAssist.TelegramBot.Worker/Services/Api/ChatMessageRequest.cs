using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public class ChatMessageRequest
{
    public required string Text { get; set; }

    public Guid? ConversationId { get; set; }

    public required Guid RequestId { get; set; }
}
