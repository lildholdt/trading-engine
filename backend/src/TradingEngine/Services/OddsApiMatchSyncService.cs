using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Domain.Registry;

namespace TradingEngine.Services;

public class OddsApiMatchSyncService(
    IOddsApiClient oddsApiClient,
    IRegistry registry,
    IOptions<ApplicationSettings> options,
    ISystemState systemState,
    ILogger<OddsApiMatchSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting event polling from OddsApi");
        
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
                    var events = await oddsApiClient.GetOdds(item.OddsApiSportsType);
                    foreach (var @event in events)
                    {  
                        await registry.TryAttachOddsApi(@event);
                    }    
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching matches from OddsApi");
            }
            
            logger.LogDebug("Next OddsApi polling in {Delay} seconds.", 
                TimeSpan.FromMilliseconds(options.Value.EventPollingIntervalInMs));
            
            // Wait before polling again
            await Task.Delay(options.Value.EventPollingIntervalInMs, cancellationToken);
        }
    }
}
