using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Services;

public class OddsAPIService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<OddsAPIService> _logger;
    private readonly string _oddsApiUrl;

    public OddsAPIService(IEventBus eventBus, IHttpClientFactory httpFactory, ILogger<OddsAPIService> logger, IOptions<ApplicationSettings> settings)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oddsApiUrl = settings?.Value?.OddsApiUrl ?? "https://some.internal.api/events";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<SportEvent>(HandleSportEventAsync);
        _logger.LogInformation("OddsAPIService subscribed to SportEvent, will POST to {Url}", _oddsApiUrl);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleSportEventAsync(SportEvent evt)
    {
        if (evt == null) return;

        try
        {
            _logger.LogInformation("OddsAPIService handling SportEvent {Id}", evt.Id);

            var client = _httpFactory.CreateClient();

            var payload = new
            {
                eventId = evt.Id,
                teams = $"{evt.Team1} vs {evt.Team2}",
                league = evt.League,
                start = evt.DateTime
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await client.PostAsync(_oddsApiUrl, content);
            resp.EnsureSuccessStatusCode();

            _logger.LogInformation("OddsAPIService notified API for event {EventId}", evt.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OddsAPIService failed to notify API for event {EventId}", evt?.Id);
        }
    }
}
