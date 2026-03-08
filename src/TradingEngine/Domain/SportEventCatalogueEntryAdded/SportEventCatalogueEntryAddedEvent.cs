using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.SportEventCatalogueEntryAdded;

public class SportEventCatalogueEntryAddedEvent : IEvent
{
    public required SportEventCatalogueEntry SportEvent { get; init; }
}