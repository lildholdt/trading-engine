using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.CreateMatch;


public class MatchPersistenceHandler(IMatchRepository repository) :  IEventHandler<MatchCreatedEvent>
{
    public async Task HandleAsync(MatchCreatedEvent @event, CancellationToken cancellationToken = default)
    { 
        await repository.SaveAsync(@event.State, cancellationToken);
    }
}