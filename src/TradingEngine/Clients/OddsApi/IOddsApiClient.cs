using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public interface IOddsApiClient
{
    Task<IReadOnlyCollection<Odds>> GetOdds(SportsType sportsType);
    Task<Odds?> GetOdds(SportsType sportsType, string eventId);
}