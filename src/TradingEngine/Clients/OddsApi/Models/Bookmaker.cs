using System.Text.Json.Serialization;

namespace TradingEngine.Clients.OddsApi.Models;

public record Bookmaker
{
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
    
    [JsonPropertyName("last_update")]
    public DateTime LastUpdate { get; init; }
    
    public List<Market> Markets { get; init; } = [];
}