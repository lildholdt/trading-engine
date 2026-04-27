using System.Text.Json.Serialization;

namespace TradingEngine.Clients.OddsApi.Models;

public record Odds
{
    public required string Id { get; init; }
    
    [JsonPropertyName("sport_key")]
    public required string SportKey { get; init; }
    
    [JsonPropertyName("sport_title")]
    public required string SportTitle { get; init; }
    
    [JsonPropertyName("commence_time")]
    public DateTime CommenceTime { get; init; }
    
    [JsonPropertyName("home_team")]
    public required string HomeTeam { get; init; }
    
    [JsonPropertyName("away_team")]
    public required string AwayTeam { get; init; }

    public List<Bookmaker> Bookmakers { get; init; } = [];
}