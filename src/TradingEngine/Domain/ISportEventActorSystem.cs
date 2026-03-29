using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public interface ISportEventActorSystem
{
    public ValueTask SendAsync(ISportEventCommand command);
    void CreateAsync(EventRegistryItem entry);
    void EndAsync(SportEventId id);
}