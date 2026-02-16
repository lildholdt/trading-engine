namespace TradingEngine.Clients.PolyMarket.Models;

public class Series
{
    public string Id { get; set; }
    public string Ticker { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string SeriesType { get; set; }
    public string Recurrence { get; set; }
    public string Image { get; set; }
    public string Icon { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public bool Archived { get; set; }
    public bool Featured { get; set; }
    public bool Restricted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public decimal Volume24hr { get; set; }
    public decimal Volume { get; set; }
    public decimal Liquidity { get; set; }
    public int CommentCount { get; set; }
    public bool RequiresTranslation { get; set; }
}