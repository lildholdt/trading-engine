using System.Collections.Concurrent;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;
using TradingEngine.Domain.Events.RegistryItemCorrelated;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Utils;

namespace TradingEngine.Services.Registry;

public class InMemoryEventRegistry(ITeamMatcher teamMatcher, IEventBus eventBus, ILogger<InMemoryEventRegistry> logger) : IEventRegistry
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

        // Find MoneyLine markets
        var hasMoneyLineMarketTypes = @event.HasMoneyLineMarketTypes;
        if (!hasMoneyLineMarketTypes) return;

        // Extract team names.
        var teams = @event.Title.Split(" vs. "); 
        if (teams.Length != 2) 
            return;
        
        var home = @event.Teams.First().Name ?? teams[0];
        var away = @event.Teams.Last().Name ?? teams[1];
                       
        // Do not consider events where the game is already started
        if (@event.StartTime < DateTime.Now)
            return;
        
        // Create registry item
        var registryItem = new EventRegistryItem
        {
            Id = SportEventId.New(),
            HomeTeam = home,
            AwayTeam = away,
            StartTime = @event.StartTime,
            PolymarketEvent = @event
        };
        
        // Add registry item
        _events.TryAdd(registryItem.Id, registryItem);
    }

    public async Task AttachOddsApi(Odds odds)
    {
        var uncorrelatedItems = _events.Values.Where(e => e.OddsApiEvent == null);
        foreach (var item in uncorrelatedItems)
        {
            // Normalization and team similarity
            var direct =  Math.Round((teamMatcher.TeamScore(odds.HomeTeam, item.HomeTeam) + 
                                      teamMatcher.TeamScore(odds.AwayTeam, item.AwayTeam)) / 2, 2);
            
            var swapped = Math.Round((teamMatcher.TeamScore(odds.HomeTeam, item.AwayTeam) + 
                                      teamMatcher.TeamScore(odds.AwayTeam, item.HomeTeam)) / 2, 2);
            
            var best = Math.Max(swapped, direct);
            if (!(best > 0.9)) continue;
            
            // Time tolerance
            var timeDifference = Math.Abs((odds.CommenceTime - item.StartTime).TotalMinutes);
            if (timeDifference > 60) continue;
            
            // Attach the OddsApi event
            item.OddsApiEvent = odds;
            
            // Log details about the correlation
            logger.LogInformation("Registry item correlated. Id={Id}, " +
                                  "PolymarketHome={PolymarketHome}, " +
                                  "PolymarketAway={PolymarketAway}, " +
                                  "OddsApiHome={OddsApiHome}, " +
                                  "OddsApiAway={OddsApiAway}, " +
                                  "Score={Score}",
                                  item.Id,
                                  item.HomeTeam,
                                  item.AwayTeam,
                                  item.OddsApiEvent.HomeTeam,
                                  item.OddsApiEvent.AwayTeam,
                                  best);
            
            // Create an event actor
            await eventBus.PublishAsync(new RegistryItemCorrelatedEvent {Item =  item});
            return;
        }
    }
}
