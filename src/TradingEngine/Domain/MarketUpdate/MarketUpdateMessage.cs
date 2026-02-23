namespace TradingEngine.Domain.MarketUpdate;

public class MarketUpdateMessage(string sportEventId) : ISportEventMessage
{
    public string SportEventId { get; init; } = sportEventId;
    public required decimal HomeOdds { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.ApplyMarketUpdate(HomeOdds);
    }
}