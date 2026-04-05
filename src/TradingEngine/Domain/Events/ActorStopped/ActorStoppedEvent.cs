using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorStopped;

/// <summary>
/// Event published when a sport event actor has stopped.
/// </summary>
public class ActorStoppedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the actor that stopped.
    /// </summary>
    public required SportEventId Id { get; init; }
}
