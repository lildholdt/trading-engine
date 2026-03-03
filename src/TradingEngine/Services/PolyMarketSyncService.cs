using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;

namespace TradingEngine.Services;

public class PolyMarketSyncService(
    IPolyMarketApiClient httpClient,
    ISportEventCatalogue catalogue,
    ILogger<PolyMarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // TODO: how to pick series id?
                await httpClient.StreamEvents(10188, @event =>
                {
                    var teams = @event.Title.Split(" vs. ");
                    if (teams.Length != 2) return;
                    var catalogueEntry = new SportEventCatalogueEntry(@event.Id)
                    {
                        StartTime = @event.StartTime,
                        League = @event.Series.First().Title,
                        Sport = @event.Tags.Last().Label,
                        Team1 = teams[0],
                        Team2 = teams[1],
                    };
                        
                    // Save to the catalogue
                    _ = catalogue.SaveAsync(catalogueEntry, cancellationToken);
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