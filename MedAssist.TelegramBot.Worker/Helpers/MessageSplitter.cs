namespace MedAssist.TelegramBot.Worker.Helpers;

public static class MessageSplitter
{
    private const int MaxLength = 4000;

    public static IReadOnlyList<string> Split(string message)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(message) || message.Length <= MaxLength)
        {
            result.Add(message);
            return result;
        }

        if (message.Contains('.'))
        {
            var sentences = message.Split('.');
            var currentPart = new System.Text.StringBuilder();

            foreach (var sentence in sentences)
            {
                string sentenceWithDot = sentence.Trim().Length > 0 ? sentence.Trim() + "." : sentence;

                if (currentPart.Length + sentenceWithDot.Length > MaxLength)
                {
                    if (currentPart.Length > 0)
                    {
                        result.Add(currentPart.ToString());
                        currentPart.Clear();
                    }
                }

                if (sentenceWithDot.Length > MaxLength)
                {
                    if (currentPart.Length > 0)
                    {
                        result.Add(currentPart.ToString());
                        currentPart.Clear();
                    }

                    var words = sentence.Split(' ');
                    var wordPart = new System.Text.StringBuilder();

                    foreach (var word in words)
                    {
                        if (wordPart.Length + word.Length + 1 > MaxLength)
                        {
                            result.Add(wordPart.ToString());
                            wordPart.Clear();
                        }

                        if (wordPart.Length > 0)
                        {
                            wordPart.Append(' ');
                        }
                        wordPart.Append(word);
                    }

                    currentPart.Append(wordPart);
                }
                else
                {
                    currentPart.Append(sentenceWithDot);
                }
            }

            if (currentPart.Length > 0)
            {
                result.Add(currentPart.ToString());
            }
        }
        else
        {
            var words = message.Split(' ');
            var currentPart = new System.Text.StringBuilder();

            foreach (var word in words)
            {
                if (currentPart.Length + word.Length + 1 > MaxLength)
                {
                    result.Add(currentPart.ToString());
                    currentPart.Clear();
                }

                if (currentPart.Length > 0)
                {
                    currentPart.Append(' ');
                }
                currentPart.Append(word);
            }

            if (currentPart.Length > 0)
            {
                result.Add(currentPart.ToString());
            }
        }

        return result;
    }
}
