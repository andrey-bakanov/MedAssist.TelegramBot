using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.User.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;
    private readonly UserStateService _userStateService;

    public RegisterCommandHandler(IMediator mediator, ITelegramBotClient telegramClient, IDataService specialityDataService, UserStateService userStateService)
    {
        _mediator = mediator;
        _telegramClient = telegramClient;
        _dataService = specialityDataService;
        _userStateService = userStateService;
    }

    public async ValueTask<Unit> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var currentState = _userStateService.GetState(command.UserId);
        if (command.CallbackQuery != null)
        {
            string? registrationParameter = command.CallbackArguments.FirstOrDefault();
            if(Int32.TryParse(registrationParameter, out int result))
            {
                if(result == 1)
                {
                    await _dataService.RegisterAsync(command.UserId, command.Username);

                    _userStateService.UpdateRegistrationState(command.UserId, true);

                    await _mediator.Send(new SetSpeciality.SetSpecialityCommand(command.UpdateItem, Enumerable.Empty<string>()));
                }

                return Unit.Value;
            }
        }
        var confirmationText = await _dataService.GetConfirmationTextAsync();
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
             {
                new[] {InlineKeyboardButton.WithCallbackData(ResourceMain.Agree, $"{BotCommandNames.RegisterCommandName} {1}") },
                new[] {InlineKeyboardButton.WithCallbackData(ResourceMain.Back, BotCommandNames.StartCommandName)}
            });

        await _telegramClient.SendMessage(command.ChatId,
            confirmationText,
            ParseMode.Html,
            protectContent: true,
            replyMarkup: inlineKeyboard);

        return Unit.Value;
    }
}