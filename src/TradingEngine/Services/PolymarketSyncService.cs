using TradingEngine.Clients.Polymarket;
using TradingEngine.Services.Registry;

namespace TradingEngine.Services;

public class PolymarketSyncService(
    IPolymarketClient httpClient,
    IEventRegistry eventRegistry,
    ILogger<PolymarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // TODO: how to pick series id?
                await httpClient.StreamEvents("213697", eventRegistry.RegisterPolymarket);
            }
            catch (Exception ex)
            {
                logger.LogError("Error polling API: {ExMessage}", ex.Message);
            }

            // Wait before polling again
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Adjust polling interval as needed
        }
    }
}