using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.Polymarket;

public class PolymarketSeries : Enumeration<PolymarketSeries>
{
    public static readonly PolymarketSeries EnglishPremierLeague = new("10188", "English Premier League");
    public static readonly PolymarketSeries WorldCupQualifiers = new("10243", "World Cup Qualifiers");
    
    public PolymarketSeries() { }
    private PolymarketSeries(string seriesId, string displayName) : base(seriesId, displayName) { }
}

