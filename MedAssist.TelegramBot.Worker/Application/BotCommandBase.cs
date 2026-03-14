using Mediator;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application;

public abstract class BotCommandBase : ICommand
{
    public abstract string Name { get; }
    public bool AutoAnswer { get; init; } = true;
    public bool RegistrationRequired { get; init; } = true;
    public Message? Message { get; }
    public CallbackQuery? CallbackQuery { get; }
    public Update UpdateItem { get; }
    public IEnumerable<string> CallbackArguments { get; } = Enumerable.Empty<string>();

    protected BotCommandBase([NotNull]Update message, IEnumerable<string> args)
    {
        Message = message.Message;
        CallbackQuery = message.CallbackQuery;
        UpdateItem = message;
        CallbackArguments = args;
    }

    protected BotCommandBase([NotNull] Update message)
    {
        Message = message.Message;
        CallbackQuery = message.CallbackQuery;
        UpdateItem = message;
    }

    public bool HasCallbackData
    {
        get
        {
            return CallbackQuery != null && CallbackArguments.Any();
        }
    }

    public long UserId
    {
        get
        {
            long? userId =  (Message?.From?.Id ?? CallbackQuery?.From?.Id);
            if (userId < 1)
            {
                throw new Exception("Не удалось определить userId");
            }

            return userId!.Value;
        }
    }

    public string Username
    {
        get
        {
            return (Message?.From?.Username ?? CallbackQuery?.From?.Username ?? string.Empty);
        }
    }

    public long ChatId
    {
        get
        {
            long chatId = Message?.Chat.Id ?? CallbackQuery?.Message?.Chat.Id ?? -1;
            
            if (chatId < 1) 
            {
                throw new Exception("Не удалось определить chatId");
            }

            return chatId;
        }
    }

    public string? Text
    {
        get
        {
            return Message?.Text ?? CallbackQuery?.Data;
        }
    }
}