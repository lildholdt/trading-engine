using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class Bookmaker : ValueObject
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public required string Name { get; init; }
    public DateTime LastUpdate { get; init; }
    
    public decimal Home => Outcomes[OutcomeType.Home];
    public decimal Away => Outcomes[OutcomeType.Away];
    public decimal Draw => Outcomes[OutcomeType.Draw];
    
    public required ImmutableDictionary<OutcomeType, decimal> Outcomes { get; init; } = ImmutableDictionary<OutcomeType, decimal>.Empty;
    
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
    /// Compares the Outcomes of this bookmaker with another bookmaker to check for changes.
    /// </summary>
    /// <param name="other">The other bookmaker to compare against.</param>
    /// <returns>True if the Outcomes have changed, otherwise false.</returns>
    public bool HasOutcomesChanged(Bookmaker other)
    {
        if (Name != other.Name || Outcomes.Count != other.Outcomes.Count)
            return true;

        foreach (var kvp in Outcomes)
        {
            if (!other.Outcomes.TryGetValue(kvp.Key, out var otherValue) || kvp.Value != otherValue)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Serializes the current bookmaker object to JSON.
    /// </summary>
    /// <returns>A JSON string representation of the current bookmaker object.</returns>
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, SerializerOptions);
    }

    /// <summary>
    /// Deserializes a JSON string into a <see cref="Bookmaker"/> instance.
    /// </summary>
    /// <param name="json">The JSON content to deserialize.</param>
    /// <returns>The deserialized <see cref="Bookmaker"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is empty or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when JSON cannot be deserialized to a <see cref="Bookmaker"/>.</exception>
    public static Bookmaker Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));

        var bookmaker = JsonSerializer.Deserialize<Bookmaker>(json, SerializerOptions);
        return bookmaker ?? throw new InvalidOperationException("Failed to deserialize Bookmaker from JSON.");
    }
    
    /// <summary>
    /// Provides the components used for equality comparison of this model.
    /// </summary>
    /// <returns>An enumerable of objects representing the components used for equality checks.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return LastUpdate;
        yield return Outcomes;

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