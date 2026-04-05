using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.OddsApi;

public class OddsApiSportsType : Enumeration<OddsApiSportsType>
{
    public static readonly OddsApiSportsType EnglishPremierLeague = new("soccer_epl", "English Premier League");
    public static readonly OddsApiSportsType WorldCupQualifiers = new("soccer_fifa_world_cup_qualifiers_europe", "World Cup Qualifiers");
    
    public OddsApiSportsType() { }
    private OddsApiSportsType(string identifier, string displayName) : base(identifier, displayName) { }
}

