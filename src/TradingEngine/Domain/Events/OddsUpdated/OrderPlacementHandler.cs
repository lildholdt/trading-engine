using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.Events.OddsUpdated;

public class OrderPlacementHandler(
    IPolymarketClient polymarketClient, 
    IEventRegistry registry,
    ILogger<OrderPlacementHandler> logger) : IEventHandler<OddsUpdatedEvent>
{
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = registry.Get(@event.Id);
        if (item == null)
        {
            logger.LogError("Couldn't place order. No registry item found for id {SportEventId}", @event.Id);
            return;
        }

        if (@event.Odds.Count == 0)
        {
            logger.LogError("Couldn't place order. No odds available");
            return;
        }

        if (item.PolymarketEvent.Id == null)
        {
            logger.LogError("Couldn't place order. Polymarket event id is null id {SportEventId}", @event.Id);
            return;
        }
        
        var polymarketEvent = await polymarketClient.GetEvent(item.PolymarketEvent.Id);
        var polymarketHomeOutcome = polymarketEvent?.MoneyLineMarkets.Get(item.HomeTeam)?.Outcome;
        var polymarketAwayOutcome = polymarketEvent?.MoneyLineMarkets.Get(item.AwayTeam)?.Outcome;
        
        var averageHome = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Home)) / @event.Odds.Count, 2);
        var averageAway = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Away)) / @event.Odds.Count, 2);
        var averageDraw = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Draw)) / @event.Odds.Count, 2);
        
        // TODO: Add logic to call order placement API in Polymarket
        
        logger.LogInformation(
            "Order has been placed. HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, " +
            "AverageHome={AverageHome}, AverageAway={AverageAway}, AverageDraw={AverageDraw}",
            item.HomeTeam,
            item.AwayTeam,
            averageHome,
            averageAway,
            averageDraw
        );
    }
}