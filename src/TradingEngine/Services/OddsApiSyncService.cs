using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Domain;

namespace TradingEngine.Services;

public class OddsApiSyncService(
    IOddsApiApiClient oddsApiClient,
    IOddsEventCatalogue oddsEventCatalogue,
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
                    var entry = new OddsEventCatalogueEntry(match.Id)
                    {
                        CommenceTime = match.CommenceTime,
                        League = match.SportTitle,
                        Sport = match.SportKey,
                        Team1 = match.HomeTeam,
                        Team2 = match.AwayTeam
                    };
                    await oddsEventCatalogue.SaveAsync(entry, cancellationToken);
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
