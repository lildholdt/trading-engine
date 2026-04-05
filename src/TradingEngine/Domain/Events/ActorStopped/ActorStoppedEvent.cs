using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorStopped;

public class ActorStoppedEvent : IEvent
{
    public required SportEventId Id { get; init; }
}
