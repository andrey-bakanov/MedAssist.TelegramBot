namespace MedAssist.TelegramBot.Worker.Application;

internal static class BotCommandNames
{
    public static readonly string StartCommandName = "/start";

    public static readonly string HelpCommandName = "/help";

    public static readonly string RegisterCommandName = "/reg";

    public static readonly string SetSpecialityCommandName = "/spec";

    public static readonly string UnregisterCommandName = "/unreg";

    public static readonly string MeCommandName = "/me";

    public static readonly string ClientsCommandName = "/clients";

    public static readonly string CreateClientCommandName = "/addclient";

    public static readonly string DeleteClientCommandName = "/delclient";

    public static readonly string SelectClientInfoCommandName = "/client";

    public static readonly string StartClientSessionCommandName = "/startclient";

    public static readonly string StopClientSessionCommandName = $"{TelegramMessageIcons.Done}Завершить";

    public static bool IsCommand(string commandName)
    {
        if(string.IsNullOrWhiteSpace(commandName))
        {
            return false;
        }

        var isCommand = (commandName?.StartsWith('/')).GetValueOrDefault(); 
        if(isCommand)
        {
            return true;
        }

        return commandName!.StartsWith(StopClientSessionCommandName);
    }
}