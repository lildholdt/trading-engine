using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Domain.Events.ActorStopped;

public class RegistryCleanupHandler(
    IEventRegistry registry,
    ILogger<RegistryCleanupHandler> logger) : IEventHandler<ActorStoppedEvent>
{
    public Task HandleAsync(ActorStoppedEvent @event, CancellationToken cancellationToken)
    {
        registry.Remove(@event.Id);
        return Task.CompletedTask;
    }
}
