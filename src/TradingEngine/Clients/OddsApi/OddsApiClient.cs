using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiClient(HttpClient httpClient, IOptions<OddsApiSettings> options) : IOddsApiClient
{
    private OddsApiSettings Settings { get; init; } = options.Value;
    
    public async Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType)
    {
        var response = await httpClient.GetAsync($"{Settings.BaseUrl}/sports/{oddsApiSportsType.Value}/odds/?apiKey={Settings.ApiKey}&regions=eu"); 
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
    }

    public async Task<Odds?> GetOdds(OddsApiSportsType oddsApiSportsType, string eventId)
    {
        var response = await httpClient.GetAsync($"{Settings.BaseUrl}/sports/{oddsApiSportsType.Value}/odds/?apiKey={Settings.ApiKey}&regions=eu&eventIds={eventId}"); 
        response.EnsureSuccessStatusCode();
        var collection = await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
        return collection.Count == 0 ? null : collection.First();
    }
}

