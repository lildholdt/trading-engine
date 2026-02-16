namespace TradingEngine.Clients.PolyMarket.Models;

public class SportEntry
{
    public int Id { get; set; }
    public string Sport { get; set; }
    public string Image { get; set; }
    public string Resolution { get; set; }
    public string Ordering { get; set; }
    public string Tags { get; set; } // Tags as a list of integers
    public string Series { get; set; }
    public DateTime CreatedAt { get; set; }
}