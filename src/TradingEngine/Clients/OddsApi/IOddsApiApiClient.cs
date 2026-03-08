// Importing the namespace containing the models used in the PolyMarket API client.
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.OddsApi
{
    public interface IOddsApiApiClient
    {
        Task<IEnumerable<Match>> GetAllMatches();
        Task<MatchOdds> GetOddsForMatch(string matchId);
    }
}