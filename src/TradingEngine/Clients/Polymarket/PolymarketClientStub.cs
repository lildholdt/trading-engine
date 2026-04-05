using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

public class PolymarketClientStub : IPolymarketClient
{
    private readonly IEnumerable<Event> _events;
    
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

    public Task<IEnumerable<SportEntry>> GetSports()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetEvents(PolymarketSeries series)
    {
        return Task.FromResult(_events);
    }
    
    public Task<Event?> GetEvent(string eventId)
    {
        return Task.FromResult(_events.FirstOrDefault(e => e.Id == eventId));
    }

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

