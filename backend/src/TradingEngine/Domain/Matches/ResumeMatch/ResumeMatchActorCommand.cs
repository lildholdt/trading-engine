namespace TradingEngine.Domain.Matches.PauseMatch;

public class ResumeMatchActorCommand : IMatchCommand
{
    public required MatchId MatchId { get; init; }

    public async Task ApplyAsync(MatchActor actor)
    {
        await actor.ResumeAsync();
    }
}
