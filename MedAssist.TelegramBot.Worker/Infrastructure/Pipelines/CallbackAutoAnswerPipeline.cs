using MedAssist.TelegramBot.Worker.Application;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Infrastructure.Pipelines;

public sealed class CallbackAutoAnswerPipeline<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : BotCommandBase
{
    private readonly ITelegramBotClient _telegramClient;

    public CallbackAutoAnswerPipeline(ITelegramBotClient telegramClient)
    {
        _telegramClient = telegramClient;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if(message.AutoAnswer && message.CallbackQuery != null)
        {
            await _telegramClient.AnswerCallbackQuery(message.CallbackQuery!.Id, cancellationToken: cancellationToken);
        }

        var response = await next(message, cancellationToken);
        return response;
    }
}