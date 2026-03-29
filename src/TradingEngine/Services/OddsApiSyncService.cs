using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Services.Registry;

namespace TradingEngine.Services;

public class OddsApiSyncService(
    IOddsApiClient oddsApiClient,
    IEventRegistry eventRegistry,
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
                var events = await oddsApiClient.GetOdds();
                foreach (var @event in events)
                {  
                    await eventRegistry.AttachOddsApi(@event);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching matches from OddsApi");
            }
            
            logger.LogInformation("Next OddsApi polling in {Delay} seconds.", 
                TimeSpan.FromMilliseconds(options.Value.EventPollingIntervalInMs));
            
            // Wait before polling again
            await Task.Delay(options.Value.EventPollingIntervalInMs, cancellationToken);
        }
    }
}
