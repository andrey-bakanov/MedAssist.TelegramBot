using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Client.ListClients;

public class ListClientsCommandCommandHandler : ICommandHandler<ListClientsCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public ListClientsCommandCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(ListClientsCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);

        var clients = (await _dataService.GetClientsAsync(command.UserId)).Chunk(2);
        var inlineKeyboard = new InlineKeyboardMarkup(
                clients
                    .Select(x => x.Select(client => InlineKeyboardButton.WithCallbackData(client.Nickname, $"{BotCommandNames.SelectClientInfoCommandName} {client.Id}")).ToList())
                    .ToList()
            );

        inlineKeyboard.AddNewRow( new InlineKeyboardButton(Resources.ResourceMain.Create, $"{BotCommandNames.CreateClientCommandName}"));

        if(userState!.ClientName != null)
        {
            inlineKeyboard.AddNewRow(new InlineKeyboardButton(Resources.ResourceMain.Patient_ResetSession, $"{BotCommandNames.StartClientSessionCommandName}"));
        }

        await _telegramClient.SendMessage(command.ChatId,
            Resources.ResourceMain.PatientList,
            replyMarkup: inlineKeyboard);

        return Unit.Value;
    }
}