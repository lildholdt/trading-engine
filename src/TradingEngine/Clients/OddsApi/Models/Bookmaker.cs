using System.Text.Json.Serialization;

namespace TradingEngine.Clients.OddsApi.Models;

public record Bookmaker
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("last_update")]
    public DateTime LastUpdate { get; set; }
    
    public List<Market> Markets { get; set; }
}