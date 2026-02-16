using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Services;

public class PolyMarketSyncService(
    IPolyMarketApiClient httpClient,
    IDispatcher dispatcher, 
    ILogger<PolyMarketSyncService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // TODO: how to pick series id?
                await httpClient.StreamEvents(10188, @event =>
                {
                    var teams = @event.Title.Split(" vs. ");
                    if (teams.Length != 2) return;
                    var eventData = new SportEventDataAvailable
                    {
                        Id = @event.Id,
                        DateTime = @event.StartTime,
                        League = @event.Series.First().Title,
                        Sport = @event.Tags.Last().Label,
                        Team1 = teams[0],
                        Team2 = teams[1],
                    };

                    dispatcher.Enqueue(eventData, stoppingToken);
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error polling API: {ExMessage}", ex.Message);
            }

            // Wait before polling again
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken); // Adjust polling interval as needed
        }
    }
}