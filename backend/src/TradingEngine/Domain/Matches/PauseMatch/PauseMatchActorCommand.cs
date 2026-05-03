namespace TradingEngine.Domain.Matches.PauseMatch;

public class PauseMatchActorCommand : IMatchCommand
{
    public required MatchId MatchId { get; init; }

    public async Task ApplyAsync(MatchActor actor)
    {
        await actor.PauseAsync();
    }
}
