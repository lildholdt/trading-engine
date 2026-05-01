using Microsoft.Extensions.Options;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Domain.Registry;

namespace TradingEngine.Services;

public class PolymarketMatchSyncService(
    IPolymarketClient httpClient,
    IRegistry registry,
    IOptions<ApplicationSettings> options,
    ISystemState systemState,
    ILogger<PolymarketMatchSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting event polling from Polymarket");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!systemState.IsRunning)
            {
                await Task.Delay(options.Value.EventPollingIntervalInMs, cancellationToken);
                continue;
            }

            try
            {
                var items = registry.GetConfiguration().Where(x => x.Active);
                foreach (var item in items)
                {
                    await httpClient.StreamEvents(item.PolymarketSeriesId, registry.RegisterPolymarket);    
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