using MedAssist.TelegramBot.Worker.Application;
using MedAssist.TelegramBot.Worker.Exceptions;
using MedAssist.TelegramBot.Worker.Models;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker;

public class TelegramWorker : BackgroundService
{
    private readonly ILogger<TelegramWorker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IMediator _mediator;
    private readonly UserStateService _userStateService;
    private readonly IDataService _dataService;
    private readonly TaskCompletionSource _taskCompletionSource = new TaskCompletionSource();

    public TelegramWorker(
        ILogger<TelegramWorker> logger, 
        ITelegramBotClient client, 
        IMediator mediator, 
        UserStateService userStateService,
        IDataService dataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botClient = client ?? throw new ArgumentNullException(nameof(client));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _userStateService = userStateService ?? throw new ArgumentNullException(nameof(userStateService));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] 
            { 
                Telegram.Bot.Types.Enums.UpdateType.Message,
                Telegram.Bot.Types.Enums.UpdateType.CallbackQuery
            } 
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,                
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );

        await _taskCompletionSource.Task;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = BotCommandFactory.CreateCommand(update, _userStateService);
        if (command != null)
        {
            _logger.LogInformation($"Received message from chat {command.ChatId}: \"{command.Text}\" {command.Username} {command.UserId}");

            var state = await _userStateService.EnsureState(command.UserId, command.ChatId, async userId =>
            {
                var userInfo = await _dataService.GetUserInfoAsync(userId);

                var isRegistered = (userInfo != null) ? true : false;

                NamedItem? clientNameItem = null;
                if (userInfo != null && userInfo.LastSelectedPatientId != null)
                {
                    clientNameItem = new NamedItem()
                    {
                        Id = userInfo.LastSelectedPatientId.ToString()!,
                        Name = userInfo.LastSelectedPatientNickname!
                    };
                }

                var state = new UserState 
                { 
                    IsRegistered = isRegistered, 
                    ClientName = clientNameItem,
                    Identity = new UserChatIdentity 
                    { 
                        ChatId = command.ChatId, 
                        UserId = userId 
                    } 
                };

                return state;
            });

            try
            {
                await _mediator.Send(command, cancellationToken);

                _userStateService.UpdateLastCommand(command.UserId, command.Name);
            }
            catch (DialogDenideException exc)
            { 
                 await _botClient.SendMessage(command.ChatId, exc.Message, cancellationToken: cancellationToken);
            }
        }
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, $"Error in polling: {exception.Message}");

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}