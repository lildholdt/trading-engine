using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.StopMatch;

/// <summary>
/// Event published when a sport event actor has stopped.
/// </summary>
public class MatchStoppedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the actor that stopped.
    /// </summary>
    public required MatchId Id { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the actor stopped.
    /// </summary>
    public required DateTime StoppedAtUtc { get; init; }
}
