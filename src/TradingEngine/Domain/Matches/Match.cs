namespace TradingEngine.Domain.Matches;

public class Match
{
    public required MatchId Id { get; init; }
    public required string HomeTeam { get; init; }
    public required string AwayTeam { get; init; }
    public required DateTime StartTime { get; init; }
}
