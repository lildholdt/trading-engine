using System.Text.Json.Serialization;

namespace TradingEngine.Clients.OddsApi.Models;

public record Market
{
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;
    
    [JsonPropertyName("last_update")]
    public DateTime LastUpdate { get; init; }
    
    [JsonPropertyName("outcomes")]
    public List<Outcome> Outcomes { get; init; } = [];
}