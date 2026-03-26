namespace TradingEngine.Domain.Commands.UpdateOdds;

public class UpdateOddsMessage : ISportEventMessage
{
    public required string SportEventId { get; init; }
    public required IReadOnlyCollection<Bookmaker> Bookmakers { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.ApplyOddsUpdate(Bookmakers);
    }
}