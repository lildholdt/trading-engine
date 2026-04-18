using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.StopMatch;

/// <summary>
/// Event published when a sport event actor has stopped.
/// </summary>
public class MatchStoppedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the actor that stopped.
    /// </summary>
    public required MatchId Id { get; init; }
}
