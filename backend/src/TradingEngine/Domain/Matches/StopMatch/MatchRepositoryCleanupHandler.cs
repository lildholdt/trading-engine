using TradingEngine.Domain.Registry;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.StopMatch;

/// <summary>
/// Handles actor stopped events by removing the corresponding event entry from the registry.
/// </summary>
public class MatchRepositoryCleanupHandler(IMatchRepository repository) : IEventHandler<MatchStoppedEvent>
{
    /// <summary>
    /// Removes the stopped actor's event from the registry.
    /// </summary>
    /// <param name="event">The actor stopped event containing the event identifier to remove.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A completed task once cleanup is finished.</returns>
    public Task HandleAsync(MatchStoppedEvent @event, CancellationToken cancellationToken)
    {
        repository.RemoveAsync(@event.Id, cancellationToken);
        return Task.CompletedTask;
    }
}
