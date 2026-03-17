using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class Bookmaker : ValueObject
{
    public required string Name { get; init; }
    public DateTime LastUpdate { get; init; }
    public required Dictionary<OutcomeType, decimal> Outcomes { get; init; } = new();
}

public enum OutcomeType {
    Home,
    Away,
    Draw
}