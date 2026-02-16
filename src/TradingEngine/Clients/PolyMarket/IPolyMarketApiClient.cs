using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.PolyMarket;

public interface IPolyMarketApiClient
{
    Task<IEnumerable<SportEntry>> GetSports();
    Task<IEnumerable<Event>> GetEvents(int seriesId);
    Task StreamEvents(int seriesId, Action<Event> action);
}