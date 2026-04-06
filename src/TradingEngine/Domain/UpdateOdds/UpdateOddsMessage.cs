namespace TradingEngine.Domain.UpdateOdds;

public class UpdateOddsMessage : ISportEventMessage
{
    public required SportEventId SportEventId { get; init; }
    public required IReadOnlyCollection<Bookmaker> Bookmakers { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.ApplyOddsUpdate(Bookmakers);
    }
}