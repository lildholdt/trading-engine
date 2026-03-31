namespace TradingEngine.Domain.Messages;

public class EndMatchMessage : ISportEventMessage
{
    public required string SportEventId { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.StopAsync();
    }
}