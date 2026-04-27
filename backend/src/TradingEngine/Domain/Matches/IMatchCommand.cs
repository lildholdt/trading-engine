namespace TradingEngine.Domain.Matches;

public interface IMatchCommand
{
    public MatchId MatchId { get; init; }
    Task ApplyAsync(MatchActor actor);
}