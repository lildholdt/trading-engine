using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorCreated;

/// <summary>
/// Event published when a new sport event actor has been created.
/// </summary>
public class ActorCreatedEvent : IEvent
{
    /// <summary>
    /// Gets the initial state snapshot of the newly created actor.
    /// </summary>
    public required SportEventActorState State { get; init; }
}
