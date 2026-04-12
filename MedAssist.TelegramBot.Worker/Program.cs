using dotenv.net;
using MedAssist.TelegramBot.Worker;
using MedAssist.TelegramBot.Worker.Configuration;
using MedAssist.TelegramBot.Worker.Extensions.DependencyInjection;
using MedAssist.TelegramBot.Worker.Infrastructure;
using MedAssist.TelegramBot.Worker.Infrastructure.Pipelines;
using MedAssist.TelegramBot.Worker.Services;
using MedAssist.TelegramBot.Worker.Services.Api;
using MedAssist.TelegramBot.Worker.Services.Media;
using MedAssist.TelegramBot.Worker.Services.State;
using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Handlers;
using MedAssist.TelegramBot.Worker.Application.Bot.DialogMessage.Processing;
using Mediator;
using Microsoft.Extensions.Options;
using Refit;
using Telegram.Bot;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotEnvOptions options = new DotEnvOptions(
        overwriteExistingVars: true);
    DotEnv.Load(options);
}

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
builder.Services.AddSingleton<ICommonOcrService, CommonOcrService>();
builder.Services.AddSingleton<IMediaProcessingService, MediaProcessingService>();
builder.Services.AddSingleton<LlmResponseProcessor>();
builder.Services.AddTransient<IMessageContentHandler, TextMessageHandler>();
builder.Services.AddTransient<IMessageContentHandler, VoiceMessageHandler>();
builder.Services.AddTransient<IMessageContentHandler, DocumentMessageHandler>();
builder.Services.AddTransient<IMessageContentHandler, PhotoMessageHandler>();

builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddSingleton<TokenStorage>();

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

builder.Services
    .AddRefitClient<ICommonOcrApiClient>()
    .ConfigureHttpClient((provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<CommonOcrConfiguration>>().Value;
        client.BaseAddress = new Uri(options.Url);
    });

var host = builder.Build();
host.Run();