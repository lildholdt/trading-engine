using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public interface IOddsApiClient
{
    Task<IReadOnlyCollection<Odds>> GetOdds();
    Task<Odds?> GetOdds(string eventId);
}