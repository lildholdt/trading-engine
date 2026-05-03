using TradingEngine.Clients.Polymarket;
using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Domain.Orders.GetOrders;
using TradingEngine.Domain.Registry;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Orders;

/// <summary>
/// Handles odds updates and evaluates order placement opportunities.
/// </summary>
public class OrderPlacementHandler(
    IPolymarketClient polymarketClient,
    IEventBus eventBus,
    IRegistry registry,
    IOrdersRepository ordersRepository,
    ILogger<OrderPlacementHandler> logger) : IEventHandler<OddsUpdatedEvent>
{
    /// <summary>
    /// Processes an odds update to prepare and log order placement decisions.
    /// </summary>
    /// <param name="event">The odds update event.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = registry.Get(@event.Match.Id);
        if (item == null)
        {
            logger.LogError("Couldn't place order. No registry item found for id {SportMatchId}", @event.Match.Id);
            return;
        }

        if (@event.Match.Odds.Count == 0)
        {
            logger.LogError("Couldn't place order. No odds available");
            return;
        }

        if (item.PolymarketEvent.Id == null)
        {
            logger.LogError("Couldn't place order. Polymarket event id is null id {SportMatchId}", @event.Match.Id);
            return;
        }
        
        var averageHome = @event.Match.AverageOdds(OutcomeType.Home);
        var averageAway = @event.Match.AverageOdds(OutcomeType.Away);
        var averageDraw = @event.Match.AverageOdds(OutcomeType.Draw);
        
        var polymarketEvent = await polymarketClient.GetEvent(item.PolymarketEvent.Id);
        var markets = polymarketEvent?.MoneyLineMarkets;
        
        var homeMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.HomeTeam);
        var awayMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.AwayTeam);
        var drawMarket = markets?.FirstOrDefault(x => x.GroupItemTitle!.Contains("Draw"));
        
        var homeTokenId = homeMarket?.Outcome.ClobTokenId;
        var awayTokenId = awayMarket?.Outcome.ClobTokenId;
        var drawTokenId = drawMarket?.Outcome.ClobTokenId;
        
        var polymarketOutcomeHome = homeMarket?.Outcome.Price;
        var polymarketOutcomeAway = awayMarket?.Outcome.Price;
        var polymarketOutcomeDraw = drawMarket?.Outcome.Price;
        
        var snapshotTime = DateTime.UtcNow;
        var order = new OrderReadModel
        {
            Id = @event.Match.Id,
            SnapshotTime = snapshotTime,
            HoursBefore = (int)(item.StartTime - snapshotTime).TotalHours,
            Bookmakers = @event.Match.Odds.Select(record => new OrderReadModel.Bookmaker
            {
                Name = record.Name,
                OddsHome = record.Outcome.Home,
                OddsDraw = record.Outcome.Draw,
                OddsAway = record.Outcome.Away,
                TrueOddsHome = record.Outcome.CalculateTrueOdds(OutcomeType.Home),
                TrueOddsDraw = record.Outcome.CalculateTrueOdds(OutcomeType.Draw),
                TrueOddsAway = record.Outcome.CalculateTrueOdds(OutcomeType.Away),
            }).ToList(),
            TrueOddsAverageHome = averageHome,
            TrueOddsAverageDraw = averageDraw,
            TrueOddsAverageAway = averageAway,
            PolymarketOutcomeHome = polymarketOutcomeHome ?? 0,
            PolymarketOutcomeDraw = polymarketOutcomeDraw ?? 0,
            PolymarketOutcomeAway = polymarketOutcomeAway ?? 0,
        };

        ordersRepository.SaveOrder(order);
        
        // TODO: Add logic to call order placement API in Polymarket

        var orderPlacedEvent = new OrderPlacedEvent
        {
            Match = @event.Match
        };
        await eventBus.PublishAsync(orderPlacedEvent, cancellationToken);
        
        logger.LogInformation(
            "Order has been placed. MatchId={MatchId}, HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, " +
            "AverageHome={AverageHome}, AverageAway={AverageAway}, AverageDraw={AverageDraw}",
            item.Id,
            item.HomeTeam,
            item.AwayTeam,
            averageHome,
            averageAway,
            averageDraw
        );
    }
}