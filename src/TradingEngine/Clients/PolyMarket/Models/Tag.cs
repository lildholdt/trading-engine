namespace TradingEngine.Clients.PolyMarket.Models;

public class Tag
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Slug { get; set; }
    public bool ForceShow { get; set; }
    public string PublishedAt { get; set; }
    public int UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool ForceHide { get; set; }
    public bool RequiresTranslation { get; set; }
}