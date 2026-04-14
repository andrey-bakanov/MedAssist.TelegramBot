using MedAssist.TelegramBot.Worker.Extensions;
using MedAssist.TelegramBot.Worker.Helpers;
using MedAssist.TelegramBot.Worker.Services;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Processing;

public class LlmResponseProcessor
{
    private readonly IDataService _dataService;

    public LlmResponseProcessor(IDataService dataService)
    {
        _dataService = dataService;
    }


    public async Task<(string Message, IReadOnlyList<InlineKeyboardButton> Buttons)> ProcessAsync(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return (string.Empty, Array.Empty<InlineKeyboardButton>());
        }

        List<InlineKeyboardButton> subspecButtons = new();

        MatchCollection matches = Regex.Matches(response, @"<subspec>(.*?)</subspec>");
        if (matches.Count > 0)
        {
            var specialities = await _dataService.GetSpecialitiesAsync();
                
            foreach (Match match in matches.Take(2))
            {
                var subspec = specialities.FirstOrDefault(x => x.Code == match.Groups[1].Value);
                if (subspec != null)
                {
                    string subspecData = $"subspec_{match.Groups[1].Value}";
                    subspecButtons.Add(InlineKeyboardButton.WithCallbackData(subspec.Title, subspecData));
                }
            }
        }

        string cleanedMessage = Regex.Replace(response, @"<subspec>.*?</subspec>", string.Empty, RegexOptions.IgnoreCase).EscapeMarkdownSpecialCharacters();

        return (cleanedMessage, subspecButtons);
    }

    public IReadOnlyList<string> SplitMessage(string message)
    {
        return MessageSplitter.Split(message);
    }
}