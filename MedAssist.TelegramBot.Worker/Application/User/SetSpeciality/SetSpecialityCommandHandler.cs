using MedAssist.TelegramBot.Worker.Resources;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;

using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.User.SetSpeciality;

public class SetSpecialityCommandHandler : ICommandHandler<SetSpecialityCommand>
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _telegramClient;
    private readonly IDataService _dataService;
    private readonly UserStateService _userStateService;

    private const string SkipSpecialityValue = "-";
    public SetSpecialityCommandHandler(IMediator mediator, ITelegramBotClient telegramClient, IDataService specialityDataService, UserStateService userStateService)
    {
        _mediator = mediator;
        _telegramClient = telegramClient;
        _dataService = specialityDataService;
        _userStateService = userStateService;
    }

    public async ValueTask<Unit> Handle(SetSpecialityCommand command, CancellationToken cancellationToken)
    {
        var currentState = _userStateService.GetState(command.UserId);

        var specialities = await _dataService.GetSpecialitiesAsync();

        if (command.CallbackQuery != null)
        {
            string? speciality = command.CallbackArguments.FirstOrDefault();
            if (!String.IsNullOrEmpty(speciality))
            {
                if (speciality != SkipSpecialityValue)
                {
                    await _dataService.UpdateSpecialityAsync(command.UserId, speciality);

                    Speciality? selectedSpeciality = specialities.FirstOrDefault(x => x.Code == speciality);

                    await _telegramClient.SendMessage(
                        command.CallbackQuery.Message!.Chat!.Id,
                        $"{TelegramMessageIcons.Done} {String.Format(ResourceMain.SpecialitySelected, selectedSpeciality?.Title)} {ResourceMain.ChatReady}");
                }
                else
                {
                    await _telegramClient.SendMessage(
                        command.CallbackQuery.Message!.Chat!.Id,
                        $"{TelegramMessageIcons.Done} {ResourceMain.ChatReady}");
                }

                return Unit.Value;
            }            
        }
        var chunkedSpecialities = specialities.Chunk(2);
        var inlineKeyboard = new InlineKeyboardMarkup(
                chunkedSpecialities
                    .Select(x =>  x.Select( spec => InlineKeyboardButton.WithCallbackData(spec.Title, $"{BotCommandNames.SetSpecialityCommandName} {spec.Code}")).ToList())
                    .ToList()
            );
        inlineKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(ResourceMain.Skip, $"{BotCommandNames.SetSpecialityCommandName} {SkipSpecialityValue}"));

        await _telegramClient.SendMessage(
            command.ChatId, 
            ResourceMain.SpecialitySelectInvite, 
            replyMarkup: inlineKeyboard, 
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}