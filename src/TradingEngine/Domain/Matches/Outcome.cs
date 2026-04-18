using TradingEngine.Infrastructure;

namespace TradingEngine.Domain.Matches;

public class Outcome : ValueObject
{
    /// <summary>
    /// Gets the odds for the home outcome.
    /// </summary>
    public required decimal Home { get; init; }

    /// <summary>
    /// Gets the odds for the away outcome.
    /// </summary>
    public required decimal Away { get; init; }

    /// <summary>
    /// Gets the odds for the draw outcome.
    /// </summary>
    public decimal Draw { get; init; } = 0;
    
    /// <summary>
    /// Retrieves the odds for a specific outcome type.
    /// </summary>
    /// <param name="outcomeType">The outcome type for which odds are requested.</param>
    /// <returns>The odds associated with the specified outcome type.</returns>
    /// <exception cref="ArgumentException">Thrown if the outcome type is invalid.</exception>
    public decimal GetOdds(OutcomeType outcomeType) => outcomeType switch
    {
        OutcomeType.Home => Home,
        OutcomeType.Away => Away,
        OutcomeType.Draw => Draw,
        _ => throw new ArgumentException($"Invalid outcome type: {outcomeType}.", nameof(outcomeType))
    };
    
    /// <summary>
    /// Calculates the true odds for a specific outcome type based on market margin.
    /// </summary>
    /// <param name="outcomeType">The outcome type for which true odds are to be calculated.</param>
    /// <returns>The true odds for the specified outcome type.</returns>
    public decimal CalculateTrueOdds(OutcomeType outcomeType)
    {
        // Calculate the market margin
        var margin = 1 / Home + 1 / Away + 1 / Draw - 1;

        // Calculate and return the true odds for the specified outcome
        var rawOdds = GetOdds(outcomeType);
        return Math.Round(rawOdds / (1 + margin), 2);
    }
}