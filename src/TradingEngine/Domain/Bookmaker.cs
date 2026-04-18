using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

/// <summary>
/// Represents a bookmaker and its associated odds.
/// </summary>
public class Bookmaker : ValueObject
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Gets the name of the bookmaker.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the timestamp of the last update for this bookmaker's odds.
    /// </summary>
    public DateTime UpdatedAt { get; }

    /// <summary>
    /// Gets the odds for the home outcome.
    /// </summary>
    public Outcome Outcome { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bookmaker"/> class.
    /// </summary>
    /// <param name="name">The name of the bookmaker.</param>
    /// <param name="outcome"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when any of the odds are not positive.</exception>
    public Bookmaker(string name, Outcome outcome)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        Outcome = outcome;
    }

    /// <summary>
    /// Determines whether the odds have changed compared to another bookmaker.
    /// </summary>
    /// <param name="other">The other bookmaker to compare against.</param>
    /// <returns>True if the odds have changed, otherwise false.</returns>
    public bool HasOutcomesChanged(Bookmaker other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Name != other.Name) return true;
        return Outcome != other.Outcome;
    }

    /// <summary>
    /// Serializes the current bookmaker to JSON.
    /// </summary>
    /// <returns>A JSON string representation of the current bookmaker.</returns>
    public string Serialize() => JsonSerializer.Serialize(this, SerializerOptions);

    /// <summary>
    /// Deserializes a JSON string into a <see cref="Bookmaker"/> instance.
    /// </summary>
    /// <param name="json">The JSON content to deserialize.</param>
    /// <returns>The deserialized <see cref="Bookmaker"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is empty or invalid.</exception>
    public static Bookmaker Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));

        var bookmaker = JsonSerializer.Deserialize<Bookmaker>(json, SerializerOptions);
        return bookmaker ?? throw new InvalidOperationException("Failed to deserialize Bookmaker from JSON.");
    }

    /// <summary>
    /// Provides the components used for equality comparison.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return UpdatedAt;
        yield return Outcome;
    }
}