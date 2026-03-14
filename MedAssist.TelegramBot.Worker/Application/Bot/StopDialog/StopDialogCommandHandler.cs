using MedAssist.TelegramBot.Worker.Application.Bot.StopDialog;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;

public class StopDialogCommandHandler : ICommandHandler<StopDialogCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public StopDialogCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(StopDialogCommand command, CancellationToken cancellationToken)
    {
        await _dataService.ClearClientSessionAsync(command.UserId);

        _userStateService.UpdateConversationIdState(command.UserId, null);
        _userStateService.UpdateClientSession(command.UserId, null);
        
        await _telegramClient.SendMessage(
            command.ChatId,
            Resources.ResourceMain.Patient_SessionCompleted, 
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}