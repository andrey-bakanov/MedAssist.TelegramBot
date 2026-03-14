using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Client.SelectClient;

public class SelectClientCommandHandler : ICommandHandler<SelectClientCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public SelectClientCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
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

                inlineKeyboard.AddNewRow(new[] { InlineKeyboardButton.WithCallbackData(Resources.ResourceMain.Patient_StartSession, $"{BotCommandNames.StartClientSessionCommandName} {clientId}") });
                inlineKeyboard.AddNewRow(new[] { InlineKeyboardButton.WithCallbackData(Resources.ResourceMain.Delete, $"{BotCommandNames.DeleteClientCommandName} {clientId}") });

                StringBuilder textBuilder = new StringBuilder();
                
                textBuilder.AppendLine($"{ResourceMain.Patient}: <b>{clientInfo.Nickname}</b>");
                textBuilder.AppendLine($"{ResourceMain.Allergies}: <b>{clientInfo.Allergies ?? "-"}</b>");
                textBuilder.AppendLine($"{ResourceMain.Chronic}: <b>{clientInfo.ChronicConditions ?? "-"}</b>");
                textBuilder.AppendLine($"---");
                textBuilder.AppendLine($"<i>{ResourceMain.ClientProfileHelp}</i>");

                await _telegramClient.SendMessage(command.ChatId, textBuilder.ToString(), Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }

        return Unit.Value;
    }
}