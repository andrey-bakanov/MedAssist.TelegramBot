using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Client.StopClientSession;

public class StopClientSessionCommandHandler : ICommandHandler<StopClientSessionCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public StopClientSessionCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(StopClientSessionCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);

        if (userState?.ClientName != null)
        {
            Guid clientId = Guid.Parse(userState.ClientName.Id);
            await _dataService.CompleteClientDialog(command.UserId, clientId);
        }

        _userStateService.UpdateClientSession(command.UserId, null);
        
        await _telegramClient.SendMessage(
            command.ChatId,
            Resources.ResourceMain.Patient_SessionCompleted, 
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}