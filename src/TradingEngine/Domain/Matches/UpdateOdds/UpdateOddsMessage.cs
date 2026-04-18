namespace TradingEngine.Domain.Matches.UpdateOdds;

public class UpdateOddsMessage : IMatchMessage
{
    public required MatchId MatchId { get; init; }
    public required IReadOnlyCollection<Bookmaker> Bookmakers { get; init; }
    
    public async Task ApplyAsync(MatchActor actor)
    {
        await actor.ApplyOddsUpdate(Bookmakers);
    }
}