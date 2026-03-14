using System.Text.Json.Serialization;

namespace TradingEngine.Clients.OddsApi.Models;

public record Market
{
    [JsonPropertyName("key")]
    public string Key { get; init; }
    
    [JsonPropertyName("last_update")]
    public DateTime LastUpdate { get; init; }
    
    [JsonPropertyName("outcomes")]
    public List<Outcome> Outcomes { get; init; }
}