using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiClient(HttpClient httpClient, IOptions<ApplicationSettings> options) : IOddsApiClient
{
    private ApplicationSettings Settings { get; init; } = options.Value;
    
    public async Task<IEnumerable<Odds>> GetOdds()
    {
        var response = await httpClient.GetAsync($"{Settings.OddsApi.BaseUrl}/sports/soccer_epl/odds/?apiKey={Settings.OddsApi.ApiKey}&regions=eu"); 
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<IEnumerable<Odds>>() ?? [];
    }

    public Task<Odds?> GetOdds(string eventId)
    {
        throw new NotImplementedException();
    }
}

