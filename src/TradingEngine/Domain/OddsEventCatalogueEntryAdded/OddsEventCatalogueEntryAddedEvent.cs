using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public class OddsEventCatalogueEntryAddedEvent : IEvent
{
    public OddsEventCatalogueEntry OddsEvent { get; set; }
}
