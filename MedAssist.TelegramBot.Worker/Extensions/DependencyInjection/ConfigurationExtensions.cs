using MedAssist.TelegramBot.Worker.Configuration;

namespace MedAssist.TelegramBot.Worker.Extensions.DependencyInjection;

public static class ConfigurationExtensions
{
    public static void AddMedAssistConfiguration(this IServiceCollection services)
    {
        services.AddOptions<BotConfiguration>()
            .BindConfiguration(ConfigurationDefaults.BotOptionKey);

        services.AddOptions<DataServiceConfiguration>()
            .BindConfiguration(ConfigurationDefaults.DataServiceOptionKey);

        services.AddOptions<MiniAppConfiguration>()
            .BindConfiguration(ConfigurationDefaults.MiniAppOptionKey);

        services.AddOptions<AsrConfiguration>()
            .BindConfiguration(ConfigurationDefaults.AsrOptionKey);
    }
}