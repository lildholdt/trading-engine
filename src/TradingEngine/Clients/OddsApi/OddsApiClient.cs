using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiClient(HttpClient httpClient, IOptions<ApplicationSettings> options) : IOddsApiClient
{
    private ApplicationSettings Settings { get; init; } = options.Value;
    
    public async Task<IReadOnlyCollection<Odds>> GetOdds()
    {
        var response = await httpClient.GetAsync($"{Settings.OddsApi.BaseUrl}/sports/soccer_epl/odds/?apiKey={Settings.OddsApi.ApiKey}&regions=eu"); 
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
    }

    public async Task<Odds?> GetOdds(string eventId)
    {
        var response = await httpClient.GetAsync($"{Settings.OddsApi.BaseUrl}/sports/soccer_epl/odds/?apiKey={Settings.OddsApi.ApiKey}&regions=eu&eventIds={eventId}"); 
        response.EnsureSuccessStatusCode();
        var collection = await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
        return collection.Count == 0 ? null : collection.First();
    }
}

