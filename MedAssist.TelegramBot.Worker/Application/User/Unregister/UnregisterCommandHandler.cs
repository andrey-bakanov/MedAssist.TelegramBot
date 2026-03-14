using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.User.Unregister;

public class UnregisterCommandHandler : ICommandHandler<UnregisterCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;
    private readonly UserStateService _userStateService;

    public UnregisterCommandHandler(ITelegramBotClient telegramClient, IDataService dataService, UserStateService userStateService)
    {
        _telegramClient = telegramClient;
        _dataService = dataService;
        _userStateService = userStateService;
    }

    public async ValueTask<Unit> Handle(UnregisterCommand command, CancellationToken cancellationToken)
    {
        var currentState = _userStateService.GetState(command.UserId);
        if (command.CallbackQuery != null)
        {
            string? registrationParameter = command.CallbackArguments.FirstOrDefault();
            if (Int32.TryParse(registrationParameter, out int result))
            {
                if (result == 1)
                {
                    await _dataService.UnregisterAsync(command.UserId);

                    _userStateService.UpdateRegistrationState(command.UserId, false);

                    await _telegramClient.SendMessage(command.ChatId,
                        ResourceMain.DeleteUserCompleted);
                }

                return Unit.Value;
            }
        }

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
             {
                new[] {InlineKeyboardButton.WithCallbackData(ResourceMain.DeleteUser, $"{BotCommandNames.UnregisterCommandName} 1" )},
                new[] {InlineKeyboardButton.WithCallbackData(ResourceMain.StartMenu, BotCommandNames.StartCommandName)}
            });

        await _telegramClient.SendMessage(command.ChatId,
            ResourceMain.DeleteUserWarning,
            replyMarkup: inlineKeyboard);

        return Unit.Value;
    }
}