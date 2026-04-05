using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Domain.Events.ActorStarted;

public class ActorStartedEventHandler(
    IHubPublisher<ActorStartedEvent> hubPublisher,
    ILogger<ActorStartedEventHandler> logger) : IEventHandler<ActorStartedEvent>
{
    public async Task HandleAsync(ActorStartedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Actor started. EventId={EventId}", @event.Id);
        await hubPublisher.PublishAsync(@event, cancellationToken);
    }
}
