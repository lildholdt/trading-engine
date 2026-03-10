namespace TradingEngine.Clients.PolyMarket.Models;

public record SportEntry
{
    public int Id { get; init; }
    public string Sport { get; init; }
    public string Image { get; init; }
    public string Resolution { get; init; }
    public string Ordering { get; init; }
    public string Tags { get; init; } // Tags as a list of integers
    public string Series { get; init; }
    public DateTime CreatedAt { get; init; }
}