namespace TradingEngine.Domain.UpdateOdds;

public class UpdateOddsMessage : ISportEventMessage
{
    public required string SportEventId { get; init; }
    public required IEnumerable<Bookmaker> Bookmakers { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.ApplyOddsUpdate(Bookmakers);
    }
}