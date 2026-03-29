using System.Collections.Concurrent;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(
    IEventBus eventBus, 
    IOddsProvider oddsProvider,
    IServiceProvider serviceProvider,
    ILogger<SportEventActorSystem> logger) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<string, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventMessage message)
    {
        _actors.TryGetValue(message.SportEventId, out var actor);
        return actor?.SendMessageAsync(message) ?? ValueTask.CompletedTask;
    }

    public void CreateAsync(EventRegistryItem entry)
    {
        _actors.GetOrAdd(entry.Id, new SportEventActor(entry.Id, entry.StartTime, eventBus, oddsProvider, serviceProvider));
        
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