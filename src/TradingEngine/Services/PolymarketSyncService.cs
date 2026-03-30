using Microsoft.Extensions.Options;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Services.Registry;

namespace TradingEngine.Services;

public class PolymarketSyncService(
    IPolymarketClient httpClient,
    IEventRegistry eventRegistry,
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
                // TODO: Identify strategy for selecting and correlation series ids across Polymarket and OddsAPI
                //await httpClient.StreamEvents("10188", eventRegistry.RegisterPolymarket);   // Premier League
                await httpClient.StreamEvents("10243", eventRegistry.RegisterPolymarket);
            }
            catch (Exception ex)
            {
                logger.LogError("Error polling API: {ExMessage}", ex.Message);
            }

            logger.LogInformation("Next polymarket polling in {Delay} seconds.", 
                TimeSpan.FromMilliseconds(options.Value.EventPollingIntervalInMs));
            
            // Wait before polling again
            await Task.Delay(options.Value.EventPollingIntervalInMs, cancellationToken); // Adjust polling interval as needed
        }
    }
}