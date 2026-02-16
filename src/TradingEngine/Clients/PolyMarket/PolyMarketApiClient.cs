using System.Text.Json;
using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.PolyMarket;

public class PolyMarketApiClient(HttpClient httpClient) : IPolyMarketApiClient
{
    private const string BaseUrl = "https://gamma-api.polymarket.com";

    public async Task<IEnumerable<SportEntry>> GetSports()
    {
        // Request sport events
        var response = await httpClient.GetAsync($"{BaseUrl}/sports");
        
        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        // Read the response content as a JSON string and ensure the result is never null
        return await response.DeserializeJsonAsync<IEnumerable<SportEntry>>() ?? [];
    }

    public async Task<IEnumerable<Event>> GetEvents(int seriesId)
    {
        // Request sport events
        var response = await httpClient.GetAsync($"{BaseUrl}/events?series_id={seriesId}&ative=true&closed=false");
        
        // Ensure the request was successful
        response.EnsureSuccessStatusCode();
        
        // Read the response content as a JSON string and ensure the result is never null
        return await response.DeserializeJsonAsync<IEnumerable<Event>>() ?? [];
    }

    public async Task StreamEvents(int seriesId, Action<Event> action)
    {
        var stream = await httpClient.GetStreamAsync($"{BaseUrl}/events?series_id={seriesId}&ative=true&closed=false");
        await foreach (var @event in JsonSerializer.DeserializeAsyncEnumerable<Event>(stream, new JsonSerializerOptions
                       {
                           PropertyNameCaseInsensitive = true
                       }))
        {
            if (@event != null) action(@event);
        }
    }
}