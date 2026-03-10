using TradingEngine.Clients.PolyMarket;

namespace TradingEngine.Services.PolyMarket;

public class PolymarketSyncService(
    IPolymarketApiClient httpClient,
    IPolymarketEventCatalogue catalogue,
    ILogger<PolymarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // TODO: how to pick series id?
                await httpClient.StreamEvents("213697", @event =>
                {
                    var teams = @event.Title.Split(" vs. ");
                    if (teams.Length != 2) return;
                        
                    // Save to the catalogue
                    catalogue.Add(@event);
                });
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