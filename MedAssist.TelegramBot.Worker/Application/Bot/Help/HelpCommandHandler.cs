using MedAssist.TelegramBot.Worker.Services;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
namespace MedAssist.TelegramBot.Worker.Application.Bot.Help;

public class HelpCommandHandler : ICommandHandler<HelpCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;

    public HelpCommandHandler(ITelegramBotClient telegramClient, IDataService dataService)
    {
        _telegramClient = telegramClient;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(HelpCommand command, CancellationToken cancellationToken)
    {
        var text = await _dataService.GetHelpTextAsync();

        await _telegramClient.SendMessage(command.ChatId, text.ToString(), ParseMode.Html, protectContent: true);

        return Unit.Value;
    }
}