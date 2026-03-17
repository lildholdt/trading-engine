namespace TradingEngine.Clients.Polymarket.Models;

public record Event
{
    public string Id { get; init; }
    public string Ticker { get; init; }
    public string Slug { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ResolutionSource { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Image { get; init; }
    public string Icon { get; init; }
    public bool Active { get; init; }
    public bool Closed { get; init; }
    public bool Archived { get; init; }
    public bool New { get; init; }
    public bool Featured { get; init; }
    public bool Restricted { get; init; }
    public decimal Liquidity { get; init; }
    public decimal Volume { get; init; }
    public decimal OpenInterest { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public double Competitive { get; init; }
    public decimal Volume24hr { get; init; }
    public decimal Volume1wk { get; init; }
    public decimal Volume1mo { get; init; }
    public decimal Volume1yr { get; init; }
    public bool EnableOrderBook { get; init; }
    public decimal LiquidityClob { get; init; }
    public bool NegRisk { get; init; }
    public string NegRiskMarketID { get; init; }
    public int CommentCount { get; init; }
    public List<Market> Markets { get; init; }
    public List<Series> Series { get; init; }
    public List<Tag> Tags { get; init; }
    public bool Cyom { get; init; }
    public bool ShowAllOutcomes { get; init; }
    public bool ShowMarketImages { get; init; }
    public bool EnableNegRisk { get; init; }
    public bool AutomaticallyActive { get; init; }
    public DateTime EventDate { get; init; }
    public DateTime StartTime { get; init; }
    public int EventWeek { get; init; }
    public string SeriesSlug { get; init; }
    public bool NegRiskAugmented { get; init; }
    public bool PendingDeployment { get; init; }
    public bool Deploying { get; init; }
    public DateTime DeployingTimestamp { get; init; }
    public long GameId { get; init; }
    public bool RequiresTranslation { get; init; }
    public List<Team> Teams { get; init; }
}