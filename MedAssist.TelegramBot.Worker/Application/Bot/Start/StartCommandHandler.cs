using MedAssist.TelegramBot.Worker.Configuration;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.Start;

public class StartCommandHandler : ICommandHandler<StartCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IOptions<MiniAppConfiguration> _miniAppOptions;

    public StartCommandHandler(ITelegramBotClient telegramClient, UserStateService userStateService, IOptions<MiniAppConfiguration> miniAppOptions)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _miniAppOptions = miniAppOptions;
    }

    public async ValueTask<Unit> Handle(StartCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);
        var inlineKeyboard = CreateKeyBoard(userState!.IsRegistered);

        string message = userState.IsRegistered ? ResourceMain.StartMenu : String.Format(ResourceMain.Wellcome, command.Username);
        await _telegramClient.SendMessage(command.ChatId,
            message,
            replyMarkup: inlineKeyboard);

        return Unit.Value;
    }

    private InlineKeyboardMarkup CreateKeyBoard(bool isRegistered)
    {
        var inlineKeyboard = new InlineKeyboardMarkup();

        if(!isRegistered)
        {
            inlineKeyboard.AddNewRow([InlineKeyboardButton.WithCallbackData(ResourceMain.Registration, BotCommandNames.RegisterCommandName)]);
        }
        else
        {
            inlineKeyboard.AddNewRow([InlineKeyboardButton.WithCallbackData(ResourceMain.MyProfile, BotCommandNames.MeCommandName)]);
            inlineKeyboard.AddNewRow([InlineKeyboardButton.WithCallbackData(ResourceMain.Patients, BotCommandNames.ClientsCommandName)]);
            inlineKeyboard.AddNewRow([InlineKeyboardButton.WithCallbackData(ResourceMain.Speciality, BotCommandNames.SetSpecialityCommandName)]);
        };

        var miniAppUrl = String.Format(_miniAppOptions.Value.Url, DateTimeOffset.Now.Ticks.ToString());
        //inlineKeyboard.AddNewRow([InlineKeyboardButton.WithCallbackData(ResourceMain.Help, BotCommandNames.HelpCommandName)]);
        inlineKeyboard.AddNewRow([InlineKeyboardButton.WithWebApp(ResourceMain.OpenMiniApp, new WebAppInfo { Url = miniAppUrl })]);

        return inlineKeyboard;
    }
}