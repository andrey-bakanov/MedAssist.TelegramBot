using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage;
using MedAssist.TelegramBot.Worker.Application.Bot.Help;
using MedAssist.TelegramBot.Worker.Application.Bot.Start;
using MedAssist.TelegramBot.Worker.Application.Bot.StartDialog;
using MedAssist.TelegramBot.Worker.Application.Bot.StopDialog;
using MedAssist.TelegramBot.Worker.Application.Bot.Unknown;
using MedAssist.TelegramBot.Worker.Application.Client.CreateClient;
using MedAssist.TelegramBot.Worker.Application.Client.DeleteClient;
using MedAssist.TelegramBot.Worker.Application.Client.ListClients;
using MedAssist.TelegramBot.Worker.Application.Client.SelectClient;
using MedAssist.TelegramBot.Worker.Application.Client.StartClientSession;
using MedAssist.TelegramBot.Worker.Application.User.Me;
using MedAssist.TelegramBot.Worker.Application.User.Register;
using MedAssist.TelegramBot.Worker.Application.User.SetSpeciality;
using MedAssist.TelegramBot.Worker.Application.User.Unregister;
using MedAssist.TelegramBot.Worker.Services.State;
using Telegram.Bot.Types;

namespace MedAssist.TelegramBot.Worker.Application;

public class BotCommandFactory
{
    public static BotCommandBase CreateCommand(Update update, UserStateService userStateService)
    {
        string? messageText = update.Message?.Text ?? update.CallbackQuery?.Data ?? string.Empty;
        if (!BotCommandNames.IsCommand(messageText))
        {
            if (update.Message?.ReplyToMessage != null)
            {
                var state = userStateService.GetState(update.Message.From!.Id);
                if (state?.AwaitingReplyMessageId == update.Message?.ReplyToMessage.MessageId)
                {
                    return CreateCommandInternal(update, state?.LastCommandName!, Array.Empty<string>());
                }
            }

            return new DialogMessageCommand(update);
        }

        string[] commandParts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string commandName = commandParts.First();
        string[] arguments = commandParts.Skip(1).ToArray();
        return CreateCommandInternal(update, commandName, arguments);
    }

    private static BotCommandBase CreateCommandInternal(Update update, string commandName, string[] arguments)
    {
        if (commandName == null)
        {
            return new UnknownCommand(update);
        }

        if (commandName == BotCommandNames.StartCommandName)
        {
            return new StartCommand(update);
        }

        if (commandName == BotCommandNames.HelpCommandName)
        {
            return new HelpCommand(update);
        }

        if (commandName == BotCommandNames.MeCommandName)
        {
            return new MeCommand(update, arguments);
        }

        if (commandName == BotCommandNames.RegisterCommandName)
        {
            
            return new RegisterCommand(update, arguments);
        }

        if (commandName == BotCommandNames.SetSpecialityCommandName)
        {
            return new SetSpecialityCommand(update, arguments);
        }

        if (commandName == BotCommandNames.UnregisterCommandName)
        {
            return new UnregisterCommand(update, arguments);
        }

        if (commandName == BotCommandNames.ClientsCommandName)
        {
            return new ListClientsCommand(update, arguments);
        }

        if (commandName == BotCommandNames.SelectClientInfoCommandName)
        {
            return new SelectClientCommand(update, arguments);
        }

        if (commandName == BotCommandNames.DeleteClientCommandName)
        {
            return new DeleteClientCommand(update, arguments);
        }

        if (commandName == BotCommandNames.StartClientSessionCommandName)
        {
            return new StartClientSessionCommand(update, arguments);
        }

        if (commandName == BotCommandNames.CreateClientCommandName)
        {
            return new CreateClientCommand(update, arguments);
        }

        if (commandName == BotCommandNames.StopDialogCommandName)
        {
            return new StopDialogCommand(update, []);
        }

        if (commandName == BotCommandNames.StartNewDialogCommandName)
        {
            return new StartDialogCommand(update, []);
        }

        return new UnknownCommand(update);
    }
}