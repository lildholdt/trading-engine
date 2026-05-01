namespace TradingEngine.Domain.Matches;

public class Match
{
    public required MatchId Id { get; init; }
    public required string HomeTeam { get; init; }
    public required string AwayTeam { get; init; }
    public required string Series { get; init; }
    public required DateTime StartTime { get; init; }
    public List<Bookmaker> Odds { get; set; } = [];

    public decimal AverageOdds(OutcomeType outcomeType) => 
        Math.Round(Odds.Sum(x => x.Outcome.CalculateTrueOdds(outcomeType)) / Odds.Count, 2);
    
}
