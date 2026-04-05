using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public interface IOddsApiClient
{
    Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType);
    Task<Odds?> GetOdds(OddsApiSportsType oddsApiSportsType, string eventId);
}