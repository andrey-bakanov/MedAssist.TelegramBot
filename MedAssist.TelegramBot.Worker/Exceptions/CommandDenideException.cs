namespace MedAssist.TelegramBot.Worker.Exceptions;

public class CommandDenideException : Exception
{
    public string CommandName { get; set; }  = string.Empty;

    public CommandDenideException(string commandName)
    {
        CommandName = commandName;
    }

    public CommandDenideException(string commandName, string? message) : base(message)
    {
        CommandName = commandName;
    }

    public CommandDenideException(string commandName, string? message, Exception? innerException) : base(message, innerException)
    {
        CommandName = commandName;
    }
}