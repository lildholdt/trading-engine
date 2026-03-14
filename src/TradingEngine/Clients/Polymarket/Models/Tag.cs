namespace TradingEngine.Clients.Polymarket.Models;

public record Tag
{
    public string Id { get; init; }
    public string Label { get; init; }
    public string Slug { get; init; }
    public bool ForceShow { get; init; }
    public string PublishedAt { get; init; }
    public int UpdatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool ForceHide { get; init; }
    public bool RequiresTranslation { get; init; }
}