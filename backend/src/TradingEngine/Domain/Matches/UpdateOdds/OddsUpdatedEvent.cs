using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.UpdateOdds;

/// <summary>
/// Event published when odds for a sport event have changed.
/// </summary>
public class OddsUpdatedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the sport event whose odds were updated.
    /// </summary>
    public required Match Match { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when this odds update event was emitted.
    /// </summary>
    public required DateTime UpdatedAtUtc { get; init; }
}