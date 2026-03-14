using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Client.CreateClient;

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand>
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;

    public CreateClientCommandHandler(
        ITelegramBotClient telegramClient, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _telegramClient = telegramClient;
        _userStateService = userStateService;
        _dataService = dataService;
    }

    public async ValueTask<Unit> Handle(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var userState = _userStateService.GetState(command.UserId);
        
        if (command.CallbackQuery != null)
        {
            var forceReplyMarkup = new ForceReplyMarkup
            {
                Selective = true,
                InputFieldPlaceholder = Resources.ResourceMain.InputPatientNamePlaceholder
            };

            Message message = await _telegramClient.SendMessage(
                chatId: command.ChatId,
                text: Resources.ResourceMain.InputPatientNamePlaceholder,
                replyMarkup: forceReplyMarkup
            );

            _userStateService.UpdateReplyMessageId(command.UserId, message.MessageId);
        }

        if (command.Message?.ReplyToMessage != null)
        {
            string clientName = command.Message.Text!;
            if(!String.IsNullOrEmpty(clientName))
            {
                //Create client
                await _dataService.CreateClientInfoAsync(command.UserId, clientName);

                await _telegramClient.SendMessage(
                    command.ChatId,
                    String.Format(Resources.ResourceMain.PatientCreated, clientName));

                _userStateService.UpdateReplyMessageId(command.UserId, null);

                return Unit.Value;
            }
            else
            {
                await _telegramClient.SendMessage(
                    command.ChatId,
                    Resources.ResourceMain.InputPatientNamePlaceholder);
            }
        }

        return Unit.Value;
    }
}