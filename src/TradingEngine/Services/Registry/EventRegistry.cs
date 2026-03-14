using System.Collections.Concurrent;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Domain;
using TradingEngine.Utils;
using PolyMarketEvent = TradingEngine.Clients.Polymarket.Models.Event;

namespace TradingEngine.Services.Registry;

public class InMemoryEventRegistry(ITeamMatcher teamMatcher, ISportEventActorSystem actorSystem) : IEventRegistry
{
    private readonly ConcurrentDictionary<SportEventId, RegistryItem> _events = new();

    public RegistryItem? Get(SportEventId id)
    {
        return _events.TryGetValue(id, out var item) ? item : null;
    }

    public void RegisterPolymarket(PolyMarketEvent @event)
    {
        var found = TryGetBySourceEvent(@event, out _);
        if (found) return;
        
        var teams = @event.Title.Split(" vs. "); // TODO: Improve
        if (teams.Length != 2) return;
                       
        var registryItem = new RegistryItem
        {
            Id = SportEventId.New(),
            HomeTeam = teams[0],
            AwayTeam = teams[1],
            StartTime = @event.StartTime,
            PolymarketEvent = @event
        };
        
        _events.TryAdd(registryItem.Id, registryItem);
    }

    public void AttachOddsApi(Odds odds)
    {
        var uncorrelatedItems = _events.Values.Where(e => e.OddsApiEvent == null);
        foreach (var item in uncorrelatedItems)
        {
            // Normalization and team similarity
            var direct = teamMatcher.MatchScore(odds.HomeTeam, item.HomeTeam) + 
                         teamMatcher.MatchScore(odds.AwayTeam, item.AwayTeam);
            
            var swapped = teamMatcher.MatchScore(odds.HomeTeam, item.AwayTeam) + 
                          teamMatcher.MatchScore(odds.AwayTeam, item.HomeTeam);
            
            var best = Math.Max(swapped, direct);
            if (!(best > 1.5)) continue;
            
            // Time tolerance
            var timeDifference = Math.Abs((odds.CommenceTime - item.StartTime).TotalMinutes);
            if (timeDifference > 60) continue;
            
            // Attach the OddsApi event
            item.OddsApiEvent = odds;
            
            // Create an event actor
            actorSystem.CreateAsync(item);
            return;
        }
    }
    
    private bool TryGetBySourceEvent(PolyMarketEvent @event, out RegistryItem? correlatedEvent)
    {
        correlatedEvent = _events.FirstOrDefault(e => e.Value.PolymarketEvent.Id == @event.Id).Value;
        return correlatedEvent  != null;
    }
}
