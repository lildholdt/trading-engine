using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

/// <summary>
/// File-backed stub implementation of <see cref="IOddsApiClient"/> for local and mock scenarios.
/// </summary>
public class OddsApiClientStub : IOddsApiClient
{
    private readonly IReadOnlyCollection<Odds> _odds;

    /// <summary>
    /// Initializes the stub by loading odds data from a JSON file.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file containing odds payloads.</param>
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

    /// <summary>
    /// Returns all odds from the in-memory stub dataset.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <returns>A read-only copy of the configured stub odds.</returns>
    public Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType)
    {
        return Task.FromResult<IReadOnlyCollection<Odds>>(new ReadOnlyCollection<Odds>(_odds.ToList()));
    }

    /// <summary>
    /// Returns odds for one event from the in-memory stub dataset.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <param name="eventId">The event identifier.</param>
    /// <returns>The matching odds entry when found; otherwise <c>null</c>.</returns>
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

