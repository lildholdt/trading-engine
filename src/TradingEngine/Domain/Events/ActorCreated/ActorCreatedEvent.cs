using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorCreated;

public class ActorCreatedEvent : IEvent
{
    public required SportEventActorState State { get; init; }
}
