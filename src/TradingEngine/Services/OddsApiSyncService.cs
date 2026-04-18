using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Services;

public class OddsApiSyncService(
    IOddsApiClient oddsApiClient,
    IMatchRegistry matchRegistry,
    IOptions<ApplicationSettings> options,
    ILogger<OddsApiSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting event polling from OddsApi");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var items = matchRegistry.GetConfiguration().Where(x => x.Active);
                foreach (var item in items)
                {
                    var events = await oddsApiClient.GetOdds(item.OddsApiSportsType);
                    foreach (var @event in events)
                    {  
                        await matchRegistry.AttachOddsApi(@event);
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
