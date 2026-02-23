using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class SportEventCatalogueEntry(string id) : Entity<string>(id)
{
    public required DateTime StartTime { get; init; }
    public required string Sport  { get; init; }
    public required string League { get; init; }
    public required string Team1 { get; init; }
    public required string Team2 { get; init; }
    
    public IEnumerable<Market> Markets { get; init; } = new List<Market>();
}

public class Market : ValueObject
{
    public required int Id { get; init; }
    public required DateTime StartDate { get; init; }
    public required IEnumerable<MarketOutcome> Outcomes { get; init; } = new List<MarketOutcome>();
}

public class MarketOutcome : ValueObject
{
    public OutcomeType Type { get; init; }
    public decimal Odds { get; init; }
}

public enum OutcomeType
{
    Yes,
    No,
    Over,
    Under
}