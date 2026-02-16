using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Services;

public class PolyMarketSyncService(
    IPolyMarketApiClient httpClient,
    IDispatcher dispatcher, 
    ILogger<PolyMarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await httpClient.StreamEvents(10188, @event =>
                {
                    var eventCandidate = new NewSportEventDataAvailable(@event.Title, @event.StartTime);
                    dispatcher.Enqueue(eventCandidate, stoppingToken);
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error polling API: {ExMessage}", ex.Message);
            }

            // Wait before polling again
            await Task.Delay(TimeSpan.FromSeconds(200), stoppingToken); // Adjust polling interval as needed
        }
    }
}