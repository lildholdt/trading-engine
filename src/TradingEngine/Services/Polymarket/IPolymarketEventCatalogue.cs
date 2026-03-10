using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Services.PolyMarket;

public interface IPolymarketEventCatalogue
{
    Task Add(Event entry);
    IEnumerable<Event> GetAllAsync();
}