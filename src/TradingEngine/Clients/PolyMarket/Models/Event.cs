namespace TradingEngine.Clients.PolyMarket.Models;

public class Event
{
    public string Id { get; set; }
    public string Ticker { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ResolutionSource { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Image { get; set; }
    public string Icon { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public bool Archived { get; set; }
    public bool New { get; set; }
    public bool Featured { get; set; }
    public bool Restricted { get; set; }
    public decimal Liquidity { get; set; }
    public decimal Volume { get; set; }
    public decimal OpenInterest { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public double Competitive { get; set; }
    public decimal Volume24hr { get; set; }
    public decimal Volume1wk { get; set; }
    public decimal Volume1mo { get; set; }
    public decimal Volume1yr { get; set; }
    public bool EnableOrderBook { get; set; }
    public decimal LiquidityClob { get; set; }
    public bool NegRisk { get; set; }
    public string NegRiskMarketID { get; set; }
    public int CommentCount { get; set; }
    public List<Market> Markets { get; set; }
    public List<Series> Series { get; set; }
    public List<Tag> Tags { get; set; }
    public bool Cyom { get; set; }
    public bool ShowAllOutcomes { get; set; }
    public bool ShowMarketImages { get; set; }
    public bool EnableNegRisk { get; set; }
    public bool AutomaticallyActive { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime StartTime { get; set; }
    public int EventWeek { get; set; }
    public string SeriesSlug { get; set; }
    public bool NegRiskAugmented { get; set; }
    public bool PendingDeployment { get; set; }
    public bool Deploying { get; set; }
    public DateTime DeployingTimestamp { get; set; }
    public long GameId { get; set; }
    public bool RequiresTranslation { get; set; }
    public List<Team> Teams { get; set; }
}