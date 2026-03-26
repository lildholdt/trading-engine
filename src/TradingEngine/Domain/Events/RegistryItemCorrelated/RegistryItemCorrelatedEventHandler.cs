using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Events.RegistryItemCorrelated;

public class RegistryItemCorrelatedEventHandler(ISportEventActorSystem actorSystem) :  IEventHandler<RegistryItemCorrelatedEvent>
{
    public Task HandleAsync(RegistryItemCorrelatedEvent @event, CancellationToken cancellationToken = default)
    { 
        actorSystem.CreateAsync(@event.Item);
        return Task.CompletedTask;
    }
}