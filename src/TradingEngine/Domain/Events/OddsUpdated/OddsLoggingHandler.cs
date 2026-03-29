using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.Events.OddsUpdated;

public class OddsLoggingHandler(
    IOddsWriter oddsWriter,
    IPolymarketClient polymarketClient,
    IEventRegistry eventRegistry,
    ILogger<OddsLoggingHandler> logger) : IEventHandler<OddsUpdatedEvent>
{
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = eventRegistry.Get(@event.Id);
        if (item == null)
        {
            logger.LogError("Couldn't find event {id}", @event.Id);
            return;
        }
        
        var polymarketEvent = await polymarketClient.GetEvent(item.PolymarketEvent.Id);
        var markets = polymarketEvent?.GetMoneyLineMarkets();
        
        var homeMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.HomeTeam);
        var awayMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.AwayTeam);

        var homePrice = homeMarket?.OutcomePrices.ElementAt(0);
        var awayPrice = awayMarket?.OutcomePrices.ElementAt(0);
        
        var records = @event.Odds.Select(record => new OddsRecord
        {
            Id = @event.Id,
            Home = item.HomeTeam,
            Away = item.AwayTeam,
            Bookmaker = record.Name,
            StartTime = item.StartTime,
            SnapshotTime = DateTime.Now,
            HoursBefore = (int)(item.StartTime - DateTime.Now).TotalHours,
            OddsHome = record.Outcome(OutcomeType.Home),
            OddsDraw = record.Outcome(OutcomeType.Draw),
            OddsAway = record.Outcome(OutcomeType.Away),
            TrueOddsHome = record.TrueOdds(OutcomeType.Home),
            TrueOddsDraw = record.TrueOdds(OutcomeType.Draw),
            TrueOddsAway = record.TrueOdds(OutcomeType.Away),
            PolymarketOutcomeHome = homePrice ?? 0,
            PolymarketOutcomeAway = awayPrice ?? 0
        }).ToList();

        await oddsWriter.WriteRecords(records, cancellationToken);

        foreach (var record in records)
        {
            logger.LogInformation("Odds updated: {EventId}, HomeTeam: {HomeTeam}, AwayTeam: {AwayTeam}, Bookmaker: {Bookmaker}, StartTime: {StartTime}, SnapshotTime: {SnapshotTime}, HoursBefore: {HoursBefore}, TrueOddsHome: {TrueOddsHome}, TrueOddsDraw: {TrueOddsDraw}, TrueOddsAway: {TrueOddsAway}, PolymarketOutcomeHome: {PolymarketOutcomeHome}, PolymarketOutcomeAway: {PolymarketOutcomeAway}.",
                record.Id,
                record.Home,
                record.Away,
                record.Bookmaker,
                record.StartTime,
                record.SnapshotTime,
                record.HoursBefore,
                record.TrueOddsHome,
                record.TrueOddsDraw,
                record.TrueOddsAway,
                record.PolymarketOutcomeHome,
                record.PolymarketOutcomeAway);
        }
        
    }

    public record OddsRecord
    {
        public required string Id { get; init; }
        public required DateTime StartTime { get; init; }
        public required string Home { get; init; }
        public required string Away { get; init; }
        public required string Bookmaker { get; init; }
        public required DateTime SnapshotTime { get; init; }
        public required int HoursBefore { get; init; }
        public required decimal OddsHome { get; init; }
        public required decimal OddsDraw { get; init; }
        public required decimal OddsAway { get; init; }
        public required decimal TrueOddsHome { get; init; }
        public required decimal TrueOddsDraw { get; init; }
        public required decimal TrueOddsAway { get; init; }
        public required decimal PolymarketOutcomeHome { get; init; }
        public required decimal PolymarketOutcomeAway { get; init; }
    }
}