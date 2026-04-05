using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

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
        var markets = polymarketEvent?.MoneyLineMarkets;
        
        var homeMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.HomeTeam);
        var awayMarket = markets?.FirstOrDefault(x => x.GroupItemTitle == item.AwayTeam);
        var drawMarket = markets?.FirstOrDefault(x => x.GroupItemTitle!.Contains("Draw"));

        var homePrice = homeMarket?.Outcome.Price;
        var awayPrice = awayMarket?.Outcome.Price;
        var drawPrice = drawMarket?.Outcome.Price;

        var averageHome = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Home)) / @event.Odds.Count, 2);
        var averageAway = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Away)) / @event.Odds.Count, 2);
        var averageDraw = Math.Round(@event.Odds.Sum(x => x.TrueOdds(OutcomeType.Draw)) / @event.Odds.Count, 2);
        
        var records = @event.Odds.Select(record => new OddsRecord
        {
            Id = @event.Id,
            Home = item.HomeTeam,
            Away = item.AwayTeam,
            Bookmaker = record.Name,
            StartTime = item.StartTime,
            SnapshotTime = DateTime.UtcNow,
            HoursBefore = (int)(item.StartTime - DateTime.UtcNow).TotalHours,
            OddsHome = record.Outcome(OutcomeType.Home),
            OddsDraw = record.Outcome(OutcomeType.Draw),
            OddsAway = record.Outcome(OutcomeType.Away),
            TrueOddsHome = record.TrueOdds(OutcomeType.Home),
            TrueOddsDraw = record.TrueOdds(OutcomeType.Draw),
            TrueOddsAway = record.TrueOdds(OutcomeType.Away),
            TrueOddsAverageHome = averageHome,
            TrueOddsAverageDraw = averageDraw,
            TrueOddsAverageAway = averageAway,
            PolymarketOutcomeHome = homePrice ?? 0,
            PolymarketOutcomeDraw = drawPrice ?? 0,
            PolymarketOutcomeAway = awayPrice ?? 0,
        }).ToList();

        await oddsWriter.WriteRecords(records, cancellationToken);

        foreach (var record in records)
        {
            logger.LogInformation("Odds updated: {EventId}, " +
                                  "HomeTeam: {HomeTeam}, " +
                                  "AwayTeam: {AwayTeam}, " +
                                  "Bookmaker: {Bookmaker}, " +
                                  "StartTime: {StartTime}, " +
                                  "SnapshotTime: {SnapshotTime}, " +
                                  "HoursBefore: {HoursBefore}, " +
                                  "TrueOddsHome: {TrueOddsHome}, " +
                                  "TrueOddsDraw: {TrueOddsDraw}, " +
                                  "TrueOddsAway: {TrueOddsAway}, " +
                                  "TrueOddsAverageHome: {TrueOddsAverageHome}, " +
                                  "TrueOddsAverageDraw: {TrueOddsAverageDraw}, " +
                                  "TrueOddsAverageAway: {TrueOddsAverageAway}, " +
                                  "PolymarketOutcomeHome: {PolymarketOutcomeHome}, " +
                                  "PolymarketOutcomeDraw: {PolymarketOutcomeDraw}, " +
                                  "PolymarketOutcomeAway: {PolymarketOutcomeAway}.",
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
                record.TrueOddsAverageHome,
                record.TrueOddsAverageDraw,
                record.TrueOddsAverageAway,
                record.PolymarketOutcomeHome,
                record.PolymarketOutcomeDraw,
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
        public required decimal TrueOddsAverageHome { get; init; }
        public required decimal TrueOddsAverageDraw { get; init; }
        public required decimal TrueOddsAverageAway { get; init; }
        public required decimal PolymarketOutcomeHome { get; init; }
        public required decimal PolymarketOutcomeDraw { get; init; }
        public required decimal PolymarketOutcomeAway { get; init; }
    }
}