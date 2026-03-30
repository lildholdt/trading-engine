using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.OddsApi;

public class SportsType : Enumeration<SportsType>
{
    public static readonly SportsType EnglishPremierLeague = new(1, "soccer_epl", "English Premier League");
    public static readonly SportsType WorldCupQualifiers = new(2, "soccer_fifa_world_cup_qualifiers_europe", "World Cup Qualifiers");
    
    public SportsType() { }
    private SportsType(int id, string value, string displayName) : base(id, value, displayName) { }
}

