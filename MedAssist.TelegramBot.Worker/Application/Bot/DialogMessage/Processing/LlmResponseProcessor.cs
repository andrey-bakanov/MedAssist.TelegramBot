using System.Text.RegularExpressions;
using MedAssist.TelegramBot.Worker.Extensions;
using MedAssist.TelegramBot.Worker.Helpers;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Processing;

public class LlmResponseProcessor
{
    public (string Message, IReadOnlyList<InlineKeyboardButton> Buttons) Process(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return (string.Empty, Array.Empty<InlineKeyboardButton>());
        }

        List<InlineKeyboardButton> subspecButtons = new();

        MatchCollection matches = Regex.Matches(response, @"<subspec>(.*?)</subspec>");
        foreach (Match match in matches.Take(2))
        {
            string subspec = match.Groups[1].Value;
            string subspecData = $"subspec_{match.Groups[1].Value}";
            subspecButtons.Add(InlineKeyboardButton.WithCallbackData(subspec, subspecData));
        }

        string cleanedMessage = Regex.Replace(response, @"<subspec>.*?</subspec>", string.Empty, RegexOptions.IgnoreCase).EscapeMarkdownSpecialCharacters();

        return (cleanedMessage, subspecButtons);
    }

    public IReadOnlyList<string> SplitMessage(string message)
    {
        return MessageSplitter.Split(message);
    }
}