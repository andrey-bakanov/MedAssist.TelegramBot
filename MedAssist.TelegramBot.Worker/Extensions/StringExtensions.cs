namespace MedAssist.TelegramBot.Worker.Extensions;

public static class StringExtensions
{
    private static readonly string[] MarkDownSpecialChars = { "\\", "`", "*", "_", "{", "}", "[", "]", "(", ")", "#", "+", "-", ".", "!" };
    public static string EscapeMarkdownSpecialCharacters(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        text = text.Replace("**", "<br>");
        foreach (string specialChar in MarkDownSpecialChars)
        {
            text = text.Replace(specialChar, "\\" + specialChar);
        }
        text = text.Replace("<br>", "**");
        return text;
    }
}