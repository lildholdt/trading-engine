using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.ActorStarted;

public class ActorStartedEvent : IEvent
{
    public required SportEventId Id { get; init; }
}
