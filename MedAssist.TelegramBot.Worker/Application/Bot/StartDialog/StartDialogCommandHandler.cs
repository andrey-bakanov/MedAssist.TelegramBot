using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Application.Bot.StartDialog;

public class StartDialogCommandHandler : ICommandHandler<StartDialogCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public StartDialogCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(StartDialogCommand command, CancellationToken cancellationToken)
    {
        UserState? userState = _userStateService.GetState(command.UserId);

        await _dataService.ClearClientSessionAsync(command.UserId);

        userState = _userStateService.UpdateConversationIdState(command.UserId, null);
        _userStateService.UpdateClientSession(command.UserId, null);
        
        await _telegramClient.SendMessage(
            command.ChatId, 
            "Создан новый диалог без пациента. Для диалога в рамках пациента, выберете пациента в меню. Чем я могу помочь?", 
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}