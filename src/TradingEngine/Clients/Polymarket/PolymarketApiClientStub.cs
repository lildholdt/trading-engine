using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.PolyMarket;

public class PolymarketApiClientStub : IPolymarketApiClient
{
    private readonly IEnumerable<Event> _events;
    
    public PolymarketApiClientStub(string jsonFilePath)
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

    public Task<IEnumerable<SportEntry>> GetSports()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetEvents(string seriesId)
    {
        return Task.FromResult(_events);
    }

    public async Task StreamEvents(string seriesId, Action<Event> action)
    {
        // Filter events by seriesId (if applicable)
        var filteredEvents = _events.Where(e => e.Id == seriesId);

        // Simulate streaming by iterating over events with a delay
        foreach (var ev in filteredEvents)
        {
            await Task.Delay(100); // Simulate streaming delay
            action(ev); // Invoke the callback for each event
        }
    }
}

