using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// Enumeration-like value object representing supported Polymarket series.
/// </summary>
public class PolymarketSeries : Enumeration<PolymarketSeries>
{
    /// <summary>
    /// Polymarket series.
    /// </summary>
    public static readonly PolymarketSeries EnglishPremierLeague = new("10188", "English Premier League");
    public static readonly PolymarketSeries WorldCupQualifiers = new("10243", "World Cup Qualifiers");
    
    /// <summary>
    /// Initializes an empty instance for framework and reflection scenarios.
    /// </summary>
    public PolymarketSeries() { }

    /// <summary>
    /// Initializes a series with identifier and display name.
    /// </summary>
    /// <param name="seriesId">Polymarket series identifier.</param>
    /// <param name="displayName">Human-readable series name.</param>
    private PolymarketSeries(string seriesId, string displayName) : base(seriesId, displayName) { }
}

