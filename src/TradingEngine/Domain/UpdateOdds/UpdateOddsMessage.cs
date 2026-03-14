using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Domain.UpdateOdds;

public class UpdateOddsMessage : ISportEventMessage
{
    public required string SportEventId { get; init; }
    public required Odds Odds { get; init; }
    
    public async Task ApplyAsync(SportEventActor actor)
    {
        await actor.ApplyOddsUpdate(Odds);
    }
}