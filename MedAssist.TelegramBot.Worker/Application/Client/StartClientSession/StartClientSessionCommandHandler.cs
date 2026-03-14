using MedAssist.TelegramBot.Worker.Models;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Application.Client.StartClientSession;

public class StartClientSessionCommandHandler : ICommandHandler<StartClientSessionCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public StartClientSessionCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(StartClientSessionCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);

        if (command.CallbackQuery != null)
        {
            string? clientId = command.CallbackArguments.FirstOrDefault();
            if (!String.IsNullOrEmpty(clientId))
            {
                var clientInfo = await _dataService.GetClientInfoAsync(command.UserId, new Guid(clientId));
                var clientInfoItem = new NamedItem { Id = clientId, Name = clientInfo.Nickname};
                _userStateService.UpdateClientSession(command.UserId, clientInfoItem);
                _userStateService.UpdateConversationIdState(command.UserId, null);

                await _dataService.SetupClientSessionAsync(command.UserId, new Guid(clientId));

                await _telegramClient.SendMessage(
                    command.ChatId, 
                    string.Format(Resources.ResourceMain.PatientSeleted, clientInfoItem.Name), 
                    cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }

        return Unit.Value;
    }
}