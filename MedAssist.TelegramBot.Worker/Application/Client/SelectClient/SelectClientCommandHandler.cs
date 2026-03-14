using MedAssist.TelegramBot.Worker.Configuration;
using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Client.SelectClient;

public class SelectClientCommandHandler : ICommandHandler<SelectClientCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;
    private readonly IOptions<MiniAppConfiguration> _miniAppOptions;

    public SelectClientCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService,
        IOptions<MiniAppConfiguration> miniAppOptions)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
        _miniAppOptions = miniAppOptions;
    }

    public async ValueTask<Unit> Handle(SelectClientCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);

        if (command.CallbackQuery != null)
        {
            string? rawClientId = command.CallbackArguments.FirstOrDefault();
            if (!String.IsNullOrEmpty(rawClientId))
            {
                Guid clientId = Guid.Parse(rawClientId);
               var clientInfo =  await _dataService.GetClientInfoAsync(command.UserId, clientId);

                var inlineKeyboard = new InlineKeyboardMarkup();

                var miniAppUrl = String.Format(_miniAppOptions.Value.Url, DateTimeOffset.Now.Ticks.ToString());
                inlineKeyboard.AddNewRow(new[] { 
                    InlineKeyboardButton.WithCallbackData(ResourceMain.Patient_StartSession, $"{BotCommandNames.StartClientSessionCommandName} {clientId}"),
                    InlineKeyboardButton.WithWebApp(ResourceMain.OpenMiniApp, new WebAppInfo { Url = miniAppUrl })});
                inlineKeyboard.AddNewRow(new[] { InlineKeyboardButton.WithCallbackData(ResourceMain.Delete, $"{BotCommandNames.DeleteClientCommandName} {clientId}") });

                StringBuilder textBuilder = new StringBuilder();
                
                textBuilder.AppendLine($"{ResourceMain.Patient}: <b>{clientInfo.Nickname}</b>");
                textBuilder.AppendLine($"{ResourceMain.Allergies}: <b>{clientInfo.Allergies ?? "-"}</b>");
                textBuilder.AppendLine($"{ResourceMain.Chronic}: <b>{clientInfo.ChronicConditions ?? "-"}</b>");
                textBuilder.AppendLine($"---");

                
                textBuilder.AppendLine($"<i>{String.Format(ResourceMain.ClientProfileHelp, string.Empty)}</i>");

                await _telegramClient.SendMessage(command.ChatId, textBuilder.ToString(), Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }

        return Unit.Value;
    }
}