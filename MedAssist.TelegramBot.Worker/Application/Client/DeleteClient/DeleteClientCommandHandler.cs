using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

namespace MedAssist.TelegramBot.Worker.Application.Client.DeleteClient;

public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;
    private readonly UserStateService _userStateService;

    public DeleteClientCommandHandler(ITelegramBotClient telegramClient, IDataService dataService, UserStateService userStateService)
    {
        _telegramClient = telegramClient;
        _dataService = dataService;
        _userStateService = userStateService;
    }

    public async ValueTask<Unit> Handle(DeleteClientCommand command, CancellationToken cancellationToken)
    {
        var currentState = _userStateService.GetState(command.UserId);
        if (command.CallbackQuery != null)
        {
            string? rawClientId = command.CallbackArguments.FirstOrDefault();
            if (rawClientId != null)
            {
                Guid clientId = Guid.Parse(rawClientId);
                var clientInfo = await _dataService.GetClientInfoAsync(command.UserId, clientId);

                await _dataService.DeleteClientInfoAsync(command.UserId, clientId);

                await _telegramClient.SendMessage(command.ChatId,  string.Format(Resources.ResourceMain.PatientDeleted, clientInfo.Nickname));
            }
        }

        return Unit.Value;
    }
}