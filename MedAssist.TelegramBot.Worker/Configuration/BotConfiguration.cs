namespace MedAssist.TelegramBot.Worker.Configuration;

public sealed class BotConfiguration
{
    /// <summary>
    /// The webhook address
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// The secret token for webhook
    /// </summary>f
    public string? SecretToken { get; set; }

    /// <summary>
    /// The bot token
    /// </summary>
    public required string BotToken { get; set; }
}