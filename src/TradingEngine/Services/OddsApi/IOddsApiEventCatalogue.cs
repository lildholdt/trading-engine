using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Services.OddsApi;

public interface IOddsApiEventCatalogue
{
    Task Add(Match entry);
    IEnumerable<Match> GetAllAsync();
}