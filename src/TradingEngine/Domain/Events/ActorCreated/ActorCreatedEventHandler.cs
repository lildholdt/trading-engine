using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Domain.Events.ActorCreated;

public class ActorCreatedEventHandler(
    IHubPublisher<ActorCreatedEvent> hubPublisher,
    ILogger<ActorCreatedEventHandler> logger) : IEventHandler<ActorCreatedEvent>
{
    public async Task HandleAsync(ActorCreatedEvent @event, CancellationToken cancellationToken)
    {
        await hubPublisher.PublishAsync(@event, cancellationToken);
    }
}
