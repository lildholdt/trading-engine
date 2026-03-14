using System.Collections.Concurrent;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(
    ICommandBus commandBus, 
    IOddsProvider oddsProvider,
    IOrderStrategy orderStrategy,
    ILogger<SportEventActorSystem> logger) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<string, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventMessage message)
    {
        _actors.TryGetValue(message.SportEventId, out var actor);
        return actor?.SendAsync(message) ?? ValueTask.CompletedTask;
    }

    public void CreateAsync(EventRegistryItem entry)
    {
        _actors.GetOrAdd(entry.Id, new SportEventActor(entry.Id, commandBus, orderStrategy, oddsProvider));
        
        logger.LogInformation(
            "SportEventActor created. Id={Id}, HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, StartTime={StartTime},",
            entry.Id, entry.StartTime, entry.HomeTeam, entry.AwayTeam
        );
    }

    public void EndAsync(SportEventId id)
    {
        throw new NotImplementedException();
    }
}