using System.Collections.Concurrent;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Domain;

public sealed class MatchActorSystem(
    IEventBus eventBus, 
    IOddsProvider oddsProvider,
    IServiceProvider serviceProvider,
    ILogger<MatchActorSystem> logger) : IMatchActorSystem
{
    private readonly ConcurrentDictionary<MatchId, MatchActor> _actors = new();

    public ValueTask SendAsync(IMatchMessage message)
    {
        _actors.TryGetValue(message.MatchId, out var actor);
        return actor?.SendMessageAsync(message) ?? ValueTask.CompletedTask;
    }

    public MatchId CreateAsync(MatchRegistryItem entry)
    {
        var actor = new MatchActor(entry.Id, entry.HomeTeam, entry.AwayTeam, entry.StartTime, eventBus, oddsProvider, serviceProvider);
        actor.StartAsync();
        _actors.GetOrAdd(entry.Id, actor);
        return actor.GetState().Id;
    }

    public async Task StopAsync(MatchId id)
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

    public IReadOnlyCollection<MatchState> GetStates()
    {
        var sportEventActorStates = _actors.Values.Select(actor => actor.GetState()).ToArray();
        return sportEventActorStates;
    }

    public MatchState GetState(MatchId id)
    {
        if (_actors.TryGetValue(id, out var actor))
        {
            return actor.GetState();
        }

        logger.LogInformation("Couldn't get actor state. ID: {Id} was not found", id);
        throw new KeyNotFoundException($"Sport event actor with ID '{id}' was not found.");
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