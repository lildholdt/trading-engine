using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class Bookmaker : ValueObject
{
    public required string Name { get; init; }
    public DateTime LastUpdate { get; init; }
    public required Dictionary<OutcomeType, decimal> Outcomes { get; init; } = new();
    public decimal Outcome(OutcomeType outcome) => Outcomes[outcome];

    public decimal TrueOdds(OutcomeType outcomeType)
    {
        // Calculate margin
        var margin = Outcomes.Sum(outcome => 1 / outcome.Value) - 1;

        // Calculate True Odds and construct the dictionary
        return Math.Round(3 * Outcomes[outcomeType] / (3 - margin * Outcomes[outcomeType]), 2);
    }
    
    public Dictionary<OutcomeType, decimal> TrueOdds()
    {
        return Outcomes.ToDictionary(
            outcome => outcome.Key,
            outcome => TrueOdds(outcome.Key)
        );
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return LastUpdate;

        // Ensure deep equality of the dictionary by comparing key-value pairs
        foreach (var kvp in Outcomes.OrderBy(k => k.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}

public enum OutcomeType {
    Home,
    Away,
    Draw
}