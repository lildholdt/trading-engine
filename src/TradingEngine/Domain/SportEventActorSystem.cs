using System.Collections.Concurrent;
using TradingEngine.Clients.PolyMarket.Models;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.PolyMarket;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(
    ICommandBus commandBus, 
    IEventBus eventBus,
    ILogger<SportEventActorSystem> logger) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<string, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventMessage message)
    {
        _actors.TryGetValue(message.SportEventId, out var actor);
        return actor?.SendAsync(message) ?? ValueTask.CompletedTask;
    }

    public ValueTask CreateAsync(Event entry)
    {
        _actors.GetOrAdd(entry.Id, id => new SportEventActor(id, commandBus, eventBus)
        {
            StartTime = entry.StartTime,
            Sport = "entry.Sport",
            League = "entry.League",
            Team1 = "entry.Team1",
            Team2 = "entry.Team2",
        });
        
        logger.LogInformation("SportEventActor created for {EntryTeam1} vs {EntryTeam2}", "entry.Team1", "entry.Team2");
        
        return ValueTask.CompletedTask;
    }

    public ValueTask EndAsync(Event entry)
    {
        throw new NotImplementedException();
    }
}