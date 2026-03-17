using System.Collections.Concurrent;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;
using TradingEngine.Domain.CreateEvent;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Utils;

namespace TradingEngine.Services.Registry;

public class InMemoryEventRegistry(ITeamMatcher teamMatcher, ICommandBus commandBus) : IEventRegistry
{
    private readonly ConcurrentDictionary<SportEventId, EventRegistryItem> _events = new();

    public EventRegistryItem? Get(SportEventId id)
    {
        return _events.TryGetValue(id, out var item) ? item : null;
    }

    public void RegisterPolymarket(Event @event)
    {
        // Check if the event is already registered
        var found = _events.FirstOrDefault(e => e.Value.PolymarketEvent.Id == @event.Id).Value != null;
        if (found) return;
        
        // TODO: Improve how team names are extracted from Polymarket event
        var teams = @event.Title.Split(" vs. "); 
        if (teams.Length != 2) return;
                       
        var registryItem = new EventRegistryItem
        {
            Id = SportEventId.New(),
            HomeTeam = teams[0],
            AwayTeam = teams[1],
            StartTime = @event.StartTime,
            PolymarketEvent = @event
        };
        
        _events.TryAdd(registryItem.Id, registryItem);
    }

    public async Task AttachOddsApi(Odds odds)
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
            await commandBus.SendAsync(new CreateSportEventCommand {Item =  item});
            return;
        }
    }
}
