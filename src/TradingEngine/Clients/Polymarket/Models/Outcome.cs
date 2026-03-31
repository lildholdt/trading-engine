namespace TradingEngine.Clients.Polymarket.Models;

public class Outcome
{
    public string? Name { get; init; }
    public decimal Price { get; init; }
    public string? ClobTokenId { get; init; }
}