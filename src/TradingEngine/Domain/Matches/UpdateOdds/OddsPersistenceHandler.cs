using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.UpdateOdds;

/// <summary>
/// Handles odds updates and evaluates order placement opportunities.
/// </summary>
public class OddsPersistenceHandler(IMatchRepository repository, IMatchActorSystem actorSystem) : IEventHandler<OddsUpdatedEvent>
{
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var match = actorSystem.GetState(@event.Id);
        await repository.SaveAsync(match, cancellationToken);
    }
}