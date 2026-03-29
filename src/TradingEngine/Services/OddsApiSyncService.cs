using TradingEngine.Clients.OddsApi;
using TradingEngine.Services.Registry;

namespace TradingEngine.Services;

public class OddsApiSyncService(
    IOddsApiClient oddsApiClient,
    IEventRegistry eventRegistry,
    ILogger<OddsApiSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
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
            await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }
}
