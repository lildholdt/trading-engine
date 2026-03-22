using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiClientStub : IOddsApiClient
{
    private readonly IReadOnlyCollection<Odds> _odds;
    public OddsApiClientStub(string jsonFilePath)
    {
        // Load the JSON file and deserialize it into the appropriate objects
        var jsonData = File.ReadAllText(jsonFilePath);
        var parsedData = JsonSerializer.Deserialize<IReadOnlyCollection<Odds>>(jsonData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });

        // Simulate events
        _odds = parsedData ?? new List<Odds>();
    }
    public Task<IReadOnlyCollection<Odds>> GetOdds()
    {
        return Task.FromResult(_odds);
    }

    public Task<Odds?> GetOdds(string eventId)
    {
        return Task.FromResult(_odds.FirstOrDefault(e => e.Id == eventId));
    }
}

