namespace TradingEngine.Clients.Polymarket.Models;

public record Series
{
    public string Id { get; init; }
    public string Ticker { get; init; }
    public string Slug { get; init; }
    public string Title { get; init; }
    public string SeriesType { get; init; }
    public string Recurrence { get; init; }
    public string Image { get; init; }
    public string Icon { get; init; }
    public bool Active { get; init; }
    public bool Closed { get; init; }
    public bool Archived { get; init; }
    public bool Featured { get; init; }
    public bool Restricted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public decimal Volume24hr { get; init; }
    public decimal Volume { get; init; }
    public decimal Liquidity { get; init; }
    public int CommentCount { get; init; }
    public bool RequiresTranslation { get; init; }
}