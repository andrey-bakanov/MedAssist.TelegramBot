namespace MedAssist.TelegramBot.Worker.Exceptions;

public class DialogDenideException : Exception
{
    private const string UserMessage = "Невозможно начать диалог в данном контексте.";

    public DialogDenideException() : base(UserMessage)
    {

    }

    public DialogDenideException(string message) : base()
    {

    }
}