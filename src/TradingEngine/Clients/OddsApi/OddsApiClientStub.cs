using System.Collections.ObjectModel;
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
    public Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType)
    {
        return Task.FromResult<IReadOnlyCollection<Odds>>(new ReadOnlyCollection<Odds>(_odds.ToList()));
    }

    public Task<Odds?> GetOdds(OddsApiSportsType oddsApiSportsType, string eventId)
    {
        var existingOdds = _odds.FirstOrDefault(e => e.Id == eventId);

        if (existingOdds == null)
        {
            return Task.FromResult<Odds?>(null);
        }

        // Create a new instance of Odds
        var newOdds = new Odds
        {
            Id = existingOdds.Id,
            Bookmakers = existingOdds.Bookmakers,
            SportKey = existingOdds.SportKey,
            SportTitle = existingOdds.SportTitle,
            HomeTeam = existingOdds.HomeTeam,
            AwayTeam = existingOdds.AwayTeam,
        };
        return Task.FromResult<Odds?>(newOdds);
    }
}

