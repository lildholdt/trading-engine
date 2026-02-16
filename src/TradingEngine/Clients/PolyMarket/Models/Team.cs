namespace TradingEngine.Clients.PolyMarket.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string League { get; set; }
    public string Record { get; set; }
    public string Logo { get; set; }
    public string Abbreviation { get; set; }
    public string Alias { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ProviderId { get; set; }
    public string Color { get; set; }
}