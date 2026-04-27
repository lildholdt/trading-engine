using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain.Matches;
using TradingEngine.Utils;

namespace TradingEngine.Domain.Registry;

public class RegistryItem
{
    public required MatchId Id { get; init; }
    public required string HomeTeam { get; init; }
    public required string AwayTeam { get; init; }
    public required DateTime StartTime { get; init; }
    public required Event PolymarketEvent { get; init; }
    public Odds? OddsApiEvent { get; private set; } 
    public double? CorrelationScore { get; private set; }

    public bool TryAttachOddsApiEvent(ITeamMatcher teamMatcher, Odds odds)
    {
        // Only attach OddsAPI reference
        if (OddsApiEvent != null) return false;
        
        // Normalization and team similarity
        var direct =  Math.Round((teamMatcher.TeamScore(odds.HomeTeam, HomeTeam) + 
        teamMatcher.TeamScore(odds.AwayTeam, AwayTeam)) / 2, 2);
            
        var swapped = Math.Round((teamMatcher.TeamScore(odds.HomeTeam, AwayTeam) + 
        teamMatcher.TeamScore(odds.AwayTeam, HomeTeam)) / 2, 2);
            
        var best = Math.Max(swapped, direct);
        if (!(best > 0.9)) return false;
            
        // Time tolerance
        var timeDifference = Math.Abs((odds.CommenceTime - StartTime).TotalMinutes);
        if (timeDifference > 60) return false;
        
        OddsApiEvent = odds;
        CorrelationScore = Math.Round(best, 2);
        return true;
    }
}