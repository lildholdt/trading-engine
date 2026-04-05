using Microsoft.Extensions.Options;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

/// <summary>
/// HTTP implementation of <see cref="IOddsApiClient"/> for retrieving live odds data.
/// </summary>
public class OddsApiClient(HttpClient httpClient, IOptions<OddsApiSettings> options) : IOddsApiClient
{
    private OddsApiSettings Settings { get; init; } = options.Value;
    
    /// <summary>
    /// Retrieves all odds for a given sport type.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <returns>A read-only collection of odds entries.</returns>
    public async Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType)
    {
        var response = await httpClient.GetAsync($"{Settings.BaseUrl}/sports/{oddsApiSportsType.Value}/odds/?apiKey={Settings.ApiKey}&regions=eu"); 
        response.EnsureSuccessStatusCode();
        return await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
    }

    /// <summary>
    /// Retrieves odds for a specific event identifier.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The first matching odds entry when available; otherwise <c>null</c>.</returns>
    public async Task<Odds?> GetOdds(OddsApiSportsType oddsApiSportsType, string eventId)
    {
        var response = await httpClient.GetAsync($"{Settings.BaseUrl}/sports/{oddsApiSportsType.Value}/odds/?apiKey={Settings.ApiKey}&regions=eu&eventIds={eventId}"); 
        response.EnsureSuccessStatusCode();
        var collection = await response.DeserializeJsonAsync<IReadOnlyCollection<Odds>>() ?? [];
        return collection.Count == 0 ? null : collection.First();
    }
}

