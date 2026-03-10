using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiClientStub : IOddsApiApiClient
{
    private readonly IEnumerable<Match> _matches;
    public OddsApiClientStub(string jsonFilePath)
    {
        // Load the JSON file and deserialize it into the appropriate objects
        var jsonData = File.ReadAllText(jsonFilePath);
        var parsedData = JsonSerializer.Deserialize<IEnumerable<Match>>(jsonData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        });

        // Simulate events
        _matches = parsedData ?? new List<Match>();
    }
    public Task<IEnumerable<Match>> GetAllMatches()
    {
        return Task.FromResult(_matches);
    }

    public async Task<MatchOdds> GetOddsForMatch(string matchId)
    {
        throw new NotImplementedException();
    }
}

