using TradingEngine.Clients.OddsApi;

namespace TradingEngine.Services.OddsApi;

public class OddsApiSyncService(
    IOddsApiApiClient oddsApiClient,
    IOddsApiEventCatalogue oddsApiEventCatalogue,
    ILogger<OddsApiSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var matches = await oddsApiClient.GetAllMatches();
                foreach (var match in matches)
                {
                    await oddsApiEventCatalogue.Add(match);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching matches from OddsApi");
            }
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
