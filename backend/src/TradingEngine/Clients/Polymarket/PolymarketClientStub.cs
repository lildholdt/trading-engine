using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// File-backed stub implementation of <see cref="IPolymarketClient"/> for local testing.
/// </summary>
public class PolymarketClientStub : IPolymarketClient
{
    private readonly IEnumerable<Event> _events;
    
    /// <summary>
    /// Loads mock Polymarket events from a JSON file.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file containing event data.</param>
    public PolymarketClientStub(string jsonFilePath)
    {
        // Load the JSON file and deserialize it into the appropriate objects
        var jsonData = File.ReadAllText(jsonFilePath);
        var parsedData = JsonSerializer.Deserialize<IEnumerable<Event>>(jsonData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });

        // Simulate events
        _events = parsedData ?? new List<Event>();
    }

    /// <summary>
    /// Retrieves sports entries.
    /// </summary>
    /// <returns>A collection of sports entries.</returns>
    public Task<IEnumerable<SportEntry>> GetSports()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves all mock events for the requested series.
    /// </summary>
    /// <param name="series">The series to query.</param>
    public Task<IEnumerable<Event>> GetEvents(PolymarketSeries series)
    {
        return Task.FromResult(_events);
    }
    
    /// <summary>
    /// Retrieves one mock event by identifier.
    /// </summary>
    /// <param name="eventId">The event identifier.</param>
    public Task<Event?> GetEvent(string eventId)
    {
        return Task.FromResult(_events.FirstOrDefault(e => e.Id == eventId));
    }

    /// <summary>
    /// Simulates event streaming by iterating over mock events.
    /// </summary>
    /// <param name="seriesId">The series identifier (unused in stub mode).</param>
    /// <param name="action">Callback to execute for each event.</param>
    public async Task StreamEvents(string seriesId, Action<Event> action)
    {
        // Simulate streaming by iterating over events with a delay
        foreach (var ev in _events)
        {
            await Task.Delay(100); // Simulate streaming delay
            action(ev); // Invoke the callback for each event
        }
    }
}

