using TradingEngine.Clients.PolyMarket.Models;
using TradingEngine.Services.PolyMarket;

namespace TradingEngine.Domain;

public interface ISportEventActorSystem
{
    public ValueTask SendAsync(ISportEventMessage message);
    ValueTask CreateAsync(Event entry);
    ValueTask EndAsync(Event entry);
}