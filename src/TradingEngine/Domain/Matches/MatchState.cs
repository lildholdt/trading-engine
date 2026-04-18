namespace TradingEngine.Domain.Matches;

public class MatchState : Match
{
    public IReadOnlyCollection<Bookmaker> Odds { get; init; } = [];
}
