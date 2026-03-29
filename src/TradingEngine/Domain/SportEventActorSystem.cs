using System.Collections.Concurrent;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(
    IEventBus eventBus, 
    IOddsProvider oddsProvider,
    ILogger<SportEventActorSystem> logger) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<string, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventCommand command)
    {
        _actors.TryGetValue(command.SportEventId, out var actor);
        return actor?.SendMessageAsync(command) ?? ValueTask.CompletedTask;
    }

    public void CreateAsync(EventRegistryItem entry)
    {
        _actors.GetOrAdd(entry.Id, new SportEventActor(entry.Id, entry.StartTime, eventBus, oddsProvider));
        
        logger.LogInformation(
            "SportEventActor created. Id={Id}, HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, StartTime={StartTime},",
            entry.Id, entry.HomeTeam, entry.AwayTeam, entry.StartTime
        );
    }

    public void EndAsync(SportEventId id)
    {
        _actors.TryGetValue(id, out var actor);
        actor?.EndMatch();
    }
}