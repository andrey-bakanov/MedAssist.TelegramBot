namespace MedAssist.TelegramBot.Worker.Infrastructure;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) : base()
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        _logger.LogInformation($"[{requestId}] Request: {request.Method} {request.RequestUri}");

        if (request.Content != null)
        {
            var requestContent = await request.Content.ReadAsStringAsync();
            _logger.LogInformation($"[{requestId}] Request Body: {requestContent}");
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Content != null)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"[{requestId}] Response Status: {response.StatusCode} Body: {responseContent}");
        }
        else
        {
            _logger.LogInformation($"[{requestId}] Response Status: {response.StatusCode}");
        }

        return response;
    }
}