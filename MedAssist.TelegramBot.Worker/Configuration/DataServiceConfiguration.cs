namespace MedAssist.TelegramBot.Worker.Configuration;

public sealed class DataServiceConfiguration
{
    /// <summary>
    /// API url
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// API key for authentication
    /// </summary>
    public required string ApiKey { get; set; }
}