namespace TradingEngine.Domain;

public interface ISportEventActorSystem
{
    public ValueTask SendAsync(ISportEventMessage message);
    ValueTask CreateAsync(SportEventCatalogueEntry entry);
    ValueTask EndAsync(SportEventCatalogueEntry entry);
}