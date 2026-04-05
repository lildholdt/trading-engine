using System.Collections.Concurrent;
using TradingEngine.Domain.Messages;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public sealed class SportEventActorSystem(
    IEventBus eventBus, 
    IOddsProvider oddsProvider,
    IServiceProvider serviceProvider,
    ILogger<SportEventActorSystem> logger) : ISportEventActorSystem
{
    private readonly ConcurrentDictionary<SportEventId, SportEventActor> _actors = new();

    public ValueTask SendAsync(ISportEventMessage message)
    {
        _actors.TryGetValue(message.SportEventId, out var actor);
        return actor?.SendMessageAsync(message) ?? ValueTask.CompletedTask;
    }

    public void CreateAsync(EventRegistryItem entry)
    {
        var actor = new SportEventActor(entry.Id, entry.HomeTeam, entry.AwayTeam, entry.StartTime, eventBus, oddsProvider, serviceProvider);
        actor.StartAsync();
        _actors.GetOrAdd(entry.Id, actor);
    }

    public async Task StopAsync(SportEventId id)
    {
        _actors.TryGetValue(id, out var actor);
        if (actor == null)
        {
            logger.LogInformation("Couldn't stop actor. ID: {Id} was not found", id);
            return;
        }
        
        await actor.StopAsync();
        _actors.TryRemove(id, out _);
    }

    public IReadOnlyCollection<SportEventActorState> GetStates()
    {
        var sportEventActorStates = _actors.Values.Select(actor => actor.GetState()).ToArray();
        return sportEventActorStates;
    }

    public async Task Reset()
    {
        foreach (var actor in _actors)
        {
            await actor.Value.StopAsync();
            _actors.TryRemove(actor.Key, out _);
        }
    }
}