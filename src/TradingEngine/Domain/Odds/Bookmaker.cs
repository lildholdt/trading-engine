using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.Odds;

public class Bookmaker : ValueObject
{
    public required string Name { get; init; }
    public DateTime LastUpdate { get; init; }
    public required Dictionary<OutcomeType, decimal> Outcomes { get; init; } = new();
    
    /// <summary>
    /// Retrieves the odds for a specific outcome type.
    /// </summary>
    /// <param name="outcome">The outcome type for which odds are to be retrieved.</param>
    /// <returns>The decimal odds associated with the specified outcome.</returns>
    public decimal Outcome(OutcomeType outcome) => Outcomes[outcome];

    /// <summary>
    /// Calculates the "true odds" for a specific outcome type, factoring in the market margin.
    /// </summary>
    /// <param name="outcomeType">The outcome type for which true odds are to be calculated.</param>
    /// <returns>The true odds as a decimal value, rounded to two decimal places.</returns>
    public decimal TrueOdds(OutcomeType outcomeType)
    {
        // Calculate margin
        var margin = Outcomes.Sum(outcome => 1 / outcome.Value) - 1;

        // Calculate True Odds and construct the dictionary
        return Math.Round(3 * Outcomes[outcomeType] / (3 - margin * Outcomes[outcomeType]), 2);
    }
    
    /// <summary>
    /// Calculates the "true odds" for all outcomes in the model, factoring in the market margin.
    /// </summary>
    /// <returns>A dictionary mapping each <see cref="OutcomeType"/> to its calculated true odds.</returns>
    public Dictionary<OutcomeType, decimal> TrueOdds()
    {
        return Outcomes.ToDictionary(
            outcome => outcome.Key,
            outcome => TrueOdds(outcome.Key)
        );
    }
    
    /// <summary>
    /// Provides the components used for equality comparison of this model.
    /// </summary>
    /// <returns>An enumerable of objects representing the components used for equality checks.</returns>
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