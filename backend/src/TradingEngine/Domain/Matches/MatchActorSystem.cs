using System.Collections.Concurrent;
using TradingEngine.Domain.Registry;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches;

public sealed class MatchActorSystem(
    IEventBus eventBus, 
    IOddsProvider oddsProvider,
    IServiceProvider serviceProvider,
    IMatchRepository matchRepository,
    ILogger<MatchActorSystem> logger) : IMatchActorSystem
{
    private readonly ConcurrentDictionary<MatchId, MatchActor> _actors = new();

    public ValueTask SendAsync(IMatchCommand command)
    {
        _actors.TryGetValue(command.MatchId, out var actor);
        return actor?.SendMessageAsync(command) ?? ValueTask.CompletedTask;
    }

    public MatchId CreateAsync(RegistryItem entry)
    {
        // Create the match object
        var match = new Match
        {
            Id = entry.Id, 
            AwayTeam = entry.AwayTeam, 
            HomeTeam = entry.HomeTeam, 
            StartTime =  entry.StartTime
        };
        
        // Store the state of the actor
        matchRepository.SaveAsync(match);
        
        // Create and start the match actor
        var actor = new MatchActor(match, eventBus, oddsProvider, matchRepository, serviceProvider);
        actor.StartAsync();
        _actors.GetOrAdd(entry.Id, actor);
        return match.Id;
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

    public async Task Reset()
    {
        foreach (var actor in _actors)
        {
            await actor.Value.StopAsync();
            _actors.TryRemove(actor.Key, out _);
        }
    }
}