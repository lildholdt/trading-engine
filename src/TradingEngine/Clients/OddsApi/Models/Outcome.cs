namespace TradingEngine.Clients.OddsApi.Models;

public record Outcome
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}