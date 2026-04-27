namespace TradingEngine.Domain.Orders.GetOrders;

public record OrderReadModel
{
    public required string Id { get; init; }
    public required DateTime SnapshotTime { get; init; }
    public required int HoursBefore { get; init; }
    public required IReadOnlyCollection<Bookmaker> Bookmakers { get; init; }
    public required decimal TrueOddsAverageHome { get; init; }
    public required decimal TrueOddsAverageDraw { get; init; }
    public required decimal TrueOddsAverageAway { get; init; }
    public required decimal PolymarketOutcomeHome { get; init; }
    public required decimal PolymarketOutcomeDraw { get; init; }
    public required decimal PolymarketOutcomeAway { get; init; }

    public class Bookmaker
    {
        public required string Name { get; init; }
        public required decimal OddsHome { get; init; }
        public required decimal OddsDraw { get; init; }
        public required decimal OddsAway { get; init; }
        public required decimal TrueOddsHome { get; init; }
        public required decimal TrueOddsDraw { get; init; }
        public required decimal TrueOddsAway { get; init; }
    }
}

