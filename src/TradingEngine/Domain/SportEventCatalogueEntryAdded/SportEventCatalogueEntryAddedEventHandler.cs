using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Utils;

namespace TradingEngine.Domain.SportEventCatalogueEntryAdded;

public class SportEventCatalogueEntryAddedEventHandler(
    ISportEventActorSystem sportEventActorSystem,
    ITeamMatcher teamMatcher,
    ILogger<SportEventCatalogueEntryAddedEventHandler> logger) : IEventHandler<SportEventCatalogueEntryAddedEvent>
{
    public async Task HandleAsync(SportEventCatalogueEntryAddedEvent @event, CancellationToken cancellationToken)
    {
        // TODO: Correlate with Odds API
        
        
        
        await sportEventActorSystem.CreateAsync(@event.SportEvent);
    }
}