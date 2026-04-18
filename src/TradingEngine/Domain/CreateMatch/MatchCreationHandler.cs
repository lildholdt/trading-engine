using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Domain.CreateMatch;

/// <summary>
/// Handles registry correlation events by creating a sport event actor for the correlated item.
/// </summary>
public class MatchCreationHandler(IMatchActorSystem actorSystem) :  IEventHandler<RegistryItemCorrelatedEvent>
{
    /// <summary>
    /// Creates a sport event actor using the correlated registry item in the incoming event.
    /// </summary>
    /// <param name="event">The registry correlation event containing the correlated item.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A completed task once the actor creation request has been issued.</returns>
    public Task HandleAsync(RegistryItemCorrelatedEvent @event, CancellationToken cancellationToken = default)
    { 
        actorSystem.CreateAsync(@event.Item);
        return Task.CompletedTask;
    }
}