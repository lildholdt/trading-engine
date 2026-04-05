namespace TradingEngine.Clients.OddsApi.Models;

public record Outcome
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}