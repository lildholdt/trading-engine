using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorStarted;

/// <summary>
/// Event published when a sport event actor has started processing.
/// </summary>
public class ActorStartedEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the actor that started.
    /// </summary>
    public required SportEventId Id { get; init; }
}
