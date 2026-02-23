using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.SportEventCatalogueEntryAdded;

public class SportEventCatalogueEntryAdded : IEvent
{
    public required SportEventCatalogueEntry SportEvent { get; init; }
}