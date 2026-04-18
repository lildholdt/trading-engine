using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Domain.Registry;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Orders;

public class OrderLoggingHandler(
    IOddsWriter oddsWriter,
    IRegistry registry,
    ILogger<OrderLoggingHandler> logger) : IEventHandler<OrderPlacedEvent>
{
    public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = registry.Get(@event.MatchId);
        if (item == null)
        {
            logger.LogError("Couldn't find event {id}", @event.MatchId);
            return;
        }
        
        var records = @event.Odds.Select(record => new OddsRecord
        {
            Id = @event.MatchId,
            Home = item.HomeTeam,
            Away = item.AwayTeam,
            Bookmaker = record.Name,
            StartTime = item.StartTime,
            SnapshotTime = DateTime.UtcNow,
            HoursBefore = (int)(item.StartTime - DateTime.UtcNow).TotalHours,
            OddsHome = record.Outcome.Home,
            OddsDraw = record.Outcome.Draw,
            OddsAway = record.Outcome.Away,
            TrueOddsHome = record.Outcome.CalculateTrueOdds(OutcomeType.Home),
            TrueOddsDraw = record.Outcome.CalculateTrueOdds(OutcomeType.Draw),
            TrueOddsAway = record.Outcome.CalculateTrueOdds(OutcomeType.Away),
            TrueOddsAverageHome = @event.AverageHomeOdds,
            TrueOddsAverageDraw = @event.AverageDrawOdds,
            TrueOddsAverageAway = @event.AverageAwayOdds,
            PolymarketOutcomeHome = @event.HomePrice ?? 0,
            PolymarketOutcomeDraw = @event.DrawPrice ?? 0,
            PolymarketOutcomeAway = @event.AwayPrice ?? 0,
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