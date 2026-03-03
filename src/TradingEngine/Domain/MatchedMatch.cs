using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class MatchedMatch(string id) : Entity<string>(id)
{
    public string SportEventId { get; set; }
    public string OddsEventId { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public double Score1 { get; set; }
    public double Score2 { get; set; }
}
