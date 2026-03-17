using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public interface ISportEventActorSystem
{
    public ValueTask SendAsync(ISportEventMessage message);
    void CreateAsync(EventRegistryItem entry);
    void EndAsync(SportEventId id);
}