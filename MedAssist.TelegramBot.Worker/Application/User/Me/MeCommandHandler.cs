using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using Mediator;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.User.Me;

public class MeCommandHandler : ICommandHandler<MeCommand>
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;

    public MeCommandHandler(IMediator mediator, ITelegramBotClient telegramClient, IDataService dataService)
    {
        _mediator = mediator;
        _telegramClient = telegramClient;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(MeCommand command, CancellationToken cancellationToken)
    {
        var userProfile = await _dataService.GetUserInfoAsync(command.UserId);
        
        string lastSelectedUsername = string.Empty;
        if (userProfile?.LastSelectedPatientId != null)
        {
           var client = await _dataService.GetClientInfoAsync(command.UserId, userProfile.LastSelectedPatientId.Value );

            lastSelectedUsername = client.Nickname;
        }
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"Имя пользователя: <b>{userProfile?.Nickname}</b>");
        builder.AppendLine($"Специализация : <b>{userProfile?.Specializations?.FirstOrDefault()?.Title ?? "-"}</b>");
        builder.AppendLine($"Сессия пациента : <b>{lastSelectedUsername ?? "-"}</b>");
        builder.AppendLine($"Баланс : <b>{userProfile?.TokenBalance}</b>");

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(ResourceMain.Registration_Delete, BotCommandNames.UnregisterCommandName) }
        });
        await _telegramClient.SendMessage(command.ChatId,
            builder.ToString(),
            ParseMode.Html,
            replyMarkup: inlineKeyboard);

        return Unit.Value;
    }
}