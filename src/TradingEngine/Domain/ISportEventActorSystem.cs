using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public interface ISportEventActorSystem
{
    public ValueTask SendAsync(ISportEventMessage message);
    void CreateAsync(RegistryItem entry);
    void EndAsync(SportEventId id);
}