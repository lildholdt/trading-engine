namespace TradingEngine.Clients.PolyMarket.Models;

public record Team
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string League { get; init; }
    public string Record { get; init; }
    public string Logo { get; init; }
    public string Abbreviation { get; init; }
    public string Alias { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int ProviderId { get; init; }
    public string Color { get; init; }
}