using Microsoft.Extensions.Options;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Services;

public class PolymarketSyncService(
    IPolymarketClient httpClient,
    IMatchRegistry matchRegistry,
    IOptions<ApplicationSettings> options,
    ILogger<PolymarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting event polling from Polymarket");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var items = matchRegistry.GetConfiguration().Where(x => x.Active);
                foreach (var item in items)
                {
                    await httpClient.StreamEvents(item.PolymarketSeriesId, matchRegistry.RegisterPolymarket);    
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error polling API: {ExMessage}", ex.Message);
            }

            logger.LogDebug("Next polymarket polling in {Delay} seconds.", 
                TimeSpan.FromMilliseconds(options.Value.EventPollingIntervalInMs));
            
            // Wait before polling again
            await Task.Delay(options.Value.EventPollingIntervalInMs, cancellationToken); // Adjust polling interval as needed
        }
    }
}