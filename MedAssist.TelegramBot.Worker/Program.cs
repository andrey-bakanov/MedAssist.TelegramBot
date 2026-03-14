using MedAssist.TelegramBot.Worker;
using MedAssist.TelegramBot.Worker.Configuration;
using MedAssist.TelegramBot.Worker.Extensions.DependencyInjection;
using MedAssist.TelegramBot.Worker.Infrastructure;
using MedAssist.TelegramBot.Worker.Infrastructure.Pipelines;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Services.State;
using Mediator;
using Microsoft.Extensions.Options;
using Refit;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddTransient<HttpLoggingHandler>();

builder.Services.AddMemoryCache();

builder.Services.AddHostedService<TelegramWorker>();

builder.Services.AddMediator((MediatorOptions options) =>
{
    options.PipelineBehaviors = [typeof(RegistrationValidatorPipeline<,>), typeof(CallbackAutoAnswerPipeline<,>)];
});

builder.Services.AddMedAssistConfiguration();
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<UserStateService>();

builder.Services.AddSingleton<ITelegramBotClient>(
        provider =>
        {
            var options = provider.GetRequiredService<IOptions<BotConfiguration>>().Value;
            var botOptions = new TelegramBotClientOptions(options.BotToken);

            var client = new TelegramBotClient(botOptions);
            return client;
        }
    );

builder.Services
    .AddRefitClient<IAsrApiClient>()
    .ConfigureHttpClient((provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<AsrConfiguration>>().Value;
        client.BaseAddress = new Uri(options.Url);

        Console.WriteLine(options.Url);
    });

builder.Services.AddTransient<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IMedAssistApiClient>()
    .ConfigureHttpClient((provider, client) => 
        {
            var options = provider.GetRequiredService<IOptions<DataServiceConfiguration>>().Value;
            client.BaseAddress = new Uri(options.Url);
        }
    )
    .AddHttpMessageHandler<HttpLoggingHandler>()
    .AddHttpMessageHandler<AuthenticationHandler>();

builder.Services
    .AddRefitClient<IMedAssistAuthApiClient>()
    .ConfigureHttpClient((provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<DataServiceConfiguration>>().Value;
        client.BaseAddress = new Uri(options.Url);
    }
    );

builder.Services.AddSingleton<TokenStorage>();

var host = builder.Build();
host.Run();