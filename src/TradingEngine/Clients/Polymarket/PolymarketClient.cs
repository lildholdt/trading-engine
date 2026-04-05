using System.Text.Json;
using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// HTTP implementation of <see cref="IPolymarketClient"/>.
/// </summary>
public class PolymarketClient(HttpClient httpClient) : IPolymarketClient
{
    private const string BaseUrl = "https://gamma-api.polymarket.com";

    /// <summary>
    /// Retrieves available sports entries.
    /// </summary>
    public async Task<IEnumerable<SportEntry>> GetSports()
    {
        // Request sport events
        var response = await httpClient.GetAsync($"{BaseUrl}/sports");
        
        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        // Read the response content as a JSON string and ensure the result is never null
        return await response.DeserializeJsonAsync<IEnumerable<SportEntry>>() ?? [];
    }

    /// <summary>
    /// Retrieves active, open events for the specified series.
    /// </summary>
    /// <param name="series">The series to query.</param>
    public async Task<IEnumerable<Event>> GetEvents(PolymarketSeries series)
    {
        // Request sport events
        var response = await httpClient.GetAsync($"{BaseUrl}/events?series_id={series.Value}&active=true&closed=false");
        
        // Ensure the request was successful
        response.EnsureSuccessStatusCode();
        
        // Read the response content as a JSON string and ensure the result is never null
        return await response.DeserializeJsonAsync<IEnumerable<Event>>() ?? [];
    }
    
    /// <summary>
    /// Retrieves one event by identifier.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    public async Task<Event?> GetEvent(string eventId)
    {
        // Request sport events
        var response = await httpClient.GetAsync($"{BaseUrl}/events/{eventId}");
        
        // Ensure the request was successful
        response.EnsureSuccessStatusCode();
        
        // Read the response content as a JSON string and ensure the result is never null
        return await response.DeserializeJsonAsync<Event>();
    }

    /// <summary>
    /// Streams active, open events for the specified series and invokes a callback for each event.
    /// </summary>
    /// <param name="seriesId">The series identifier to stream.</param>
    /// <param name="action">Callback to execute for each streamed event.</param>
    public async Task StreamEvents(string seriesId, Action<Event> action)
    {
        var stream = await httpClient.GetStreamAsync($"{BaseUrl}/events?series_id={seriesId}&active=true&closed=false");
        await foreach (var @event in JsonSerializer.DeserializeAsyncEnumerable<Event>(stream, new JsonSerializerOptions
                       {
                           PropertyNameCaseInsensitive = true
                       }))
        {
            if (@event != null) action(@event);
        }
    }
}