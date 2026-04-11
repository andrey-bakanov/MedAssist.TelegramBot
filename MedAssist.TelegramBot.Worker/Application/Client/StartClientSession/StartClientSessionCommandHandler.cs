using MedAssist.TelegramBot.Worker.Models;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

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

        if (command.CallbackQuery == null)
        {
            return Unit.Value;
        }

        string? clientId = command.CallbackArguments.FirstOrDefault();
        if (String.IsNullOrEmpty(clientId))
        {
            return Unit.Value;
        }

        var _ = await _dataService.StartClientDialog(command.UserId, new Guid(clientId));

        var clientInfo = await _dataService.GetClientInfoAsync(command.UserId, new Guid(clientId));
        var clientInfoItem = new NamedItem { Id = clientId, Name = clientInfo.Nickname };
        _userStateService.UpdateClientSession(command.UserId, clientInfoItem);

        KeyboardButton[] keyboardButtons = [new KeyboardButton($"{BotCommandNames.StopClientSessionCommandName} [{userState.ClientName.Name}]")];
        ReplyKeyboardMarkup? keyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true,
            IsPersistent = true
        };

        await _telegramClient.SendMessage(
            command.ChatId,
            string.Format(Resources.ResourceMain.PatientSeleted, clientInfoItem.Name),
            replyMarkup: keyboardMarkup,
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}