using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.OddsApi;

/// <summary>
/// Enumeration-like value object representing supported Odds API sport keys.
/// </summary>
public class OddsApiSportsType : Enumeration<OddsApiSportsType>
{
    /// <summary>
    /// Odds API Sports types.
    /// </summary>
    public static readonly OddsApiSportsType EnglishPremierLeague = new("soccer_epl", "English Premier League");
    public static readonly OddsApiSportsType WorldCupQualifiers = new("soccer_fifa_world_cup_qualifiers_europe", "World Cup Qualifiers");
    
    /// <summary>
    /// Initializes an empty instance for framework and reflection scenarios.
    /// </summary>
    public OddsApiSportsType() { }

    /// <summary>
    /// Initializes a sport type using the Odds API identifier and display name.
    /// </summary>
    /// <param name="identifier">The Odds API sport key.</param>
    /// <param name="displayName">Human-readable sport name.</param>
    private OddsApiSportsType(string identifier, string displayName) : base(identifier, displayName) { }
}

