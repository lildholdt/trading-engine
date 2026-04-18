using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Domain.StopMatch;

/// <summary>
/// Handles actor stopped events by removing the corresponding event entry from the registry.
/// </summary>
public class RegistryCleanupHandler(IMatchRegistry registry) : IEventHandler<MatchStoppedEvent>
{
    /// <summary>
    /// Removes the stopped actor's event from the registry.
    /// </summary>
    /// <param name="event">The actor stopped event containing the event identifier to remove.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A completed task once cleanup is finished.</returns>
    public Task HandleAsync(MatchStoppedEvent @event, CancellationToken cancellationToken)
    {
        registry.Remove(@event.Id);
        return Task.CompletedTask;
    }
}
