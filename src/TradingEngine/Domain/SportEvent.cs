using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class SportEvent(string id) : Entity<string>(id)
{
    public required DateTime DateTime { get; init; }
    public required string Sport  { get; init; }
    public required string League { get; init; }
    public required string Team1 { get; init; }
    public required string Team2 { get; init; }
    public required string Market { get; init; }
    public required decimal MarketDetail { get; init; }
    public required decimal Outcome1 { get; init; }
    public required decimal Outcome2 { get; init; }
    public required decimal OutcomeX { get; init; }
    public required decimal Odds1 { get; init; }
    public required decimal Odds2 { get; init; }
    public required decimal OddsX { get; init; }
}