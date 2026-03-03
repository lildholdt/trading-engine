using System.Text.Json;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiApiClient(HttpClient httpClient) : IOddsApiApiClient
{
    private const string BaseUrl = "https://api.the-odds-api.com/v4"; 
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<Match>> GetAllMatches()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/sports/soccer_epl/odds/?apiKey=96ce8217d3d6ecf433b45a6094a87692&regions=us&,spreads&oddsFormat=american"); 
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<IEnumerable<Match>>() ?? [];
    }

    public async Task<MatchOdds> GetOddsForMatch(string matchId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/sports/soccer_epl/odds/?apiKey=96ce8217d3d6ecf433b45a6094a87692&regions=us&eventIds={matchId}&bookmakers=fanduel"); // Example endpoint
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<MatchOdds>() ?? new MatchOdds();
    }
}

