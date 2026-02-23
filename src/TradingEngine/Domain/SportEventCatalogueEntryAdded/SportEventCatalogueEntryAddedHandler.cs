using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.SportEventCatalogueEntryAdded;

public class SportEventCatalogueEntryAddedHandler(
    ISportEventActorSystem sportEventActorSystem,
    ILogger<SportEventCatalogueEntryAddedHandler> logger) : IEventHandler<SportEventCatalogueEntryAdded>
{
    public async Task HandleAsync(SportEventCatalogueEntryAdded @event, CancellationToken cancellationToken)
    {
        // TODO: Correlate with Odds API

        await sportEventActorSystem.CreateAsync(@event.SportEvent);
    }
}