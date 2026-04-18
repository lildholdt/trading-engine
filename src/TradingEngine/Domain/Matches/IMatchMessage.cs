namespace TradingEngine.Domain.Matches;

public interface IMatchMessage
{
    public MatchId MatchId { get; init; }
    Task ApplyAsync(MatchActor actor);
}