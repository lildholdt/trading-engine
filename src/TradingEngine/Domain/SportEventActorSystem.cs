using System.Collections.Concurrent;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(ICommandBus commandBus, IEventBus eventBus) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<string, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventMessage message)
    {
        _actors.TryGetValue(message.SportEventId, out var actor);
        return actor?.SendAsync(message) ?? ValueTask.CompletedTask;
    }

    public ValueTask CreateAsync(SportEventCatalogueEntry entry)
    {
        _actors.GetOrAdd(entry.Id, id => new SportEventActor(id, commandBus, eventBus)
        {
            StartTime = entry.StartTime,
            Sport = entry.Sport,
            League = entry.League,
            Team1 = entry.Team1,
            Team2 = entry.Team2,
        });
        return ValueTask.CompletedTask;
    }

    public ValueTask EndAsync(SportEventCatalogueEntry entry)
    {
        throw new NotImplementedException();
    }
}