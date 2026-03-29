namespace TradingEngine.Domain.Messages;

public class EndMatchMessage : ISportEventCommand
{
    public required string SportEventId { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.EndMatch();
    }
}