using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.UpdateOdds;

/// <summary>
/// Event published when odds for a sport event have changed.
/// </summary>
public class OddsUpdatedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the sport event whose odds were updated.
    /// </summary>
    public required SportEventId Id { get; init; }

    /// <summary>
    /// Gets the latest bookmaker odds snapshot for the event.
    /// </summary>
    public required IReadOnlyCollection<Bookmaker> Odds { get; init; }
}