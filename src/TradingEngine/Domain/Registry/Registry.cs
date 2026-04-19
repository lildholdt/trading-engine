using System.Collections.Concurrent;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain.Matches;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Utils;

namespace TradingEngine.Domain.Registry;

public class InMemoryRegistry(ITeamMatcher teamMatcher, IEventBus eventBus, ILogger<InMemoryRegistry> logger) : IRegistry
{
    private readonly ConcurrentDictionary<MatchId, RegistryItem> _events = new();
    private readonly RegistryConfiguration _configuration = new();
    
    public RegistryItem? Get(MatchId id)
    {
        return _events.TryGetValue(id, out var item) ? item : null;
    }
    
    public IReadOnlyCollection<RegistryItem> GetAll()
    {
        return _events.Values.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<RegistryMapping> GetConfiguration()
    {
        return _configuration.GetAll();
    }

    public void UpdateConfiguration(int id, bool state)
    {
        _configuration.Update(id, state);
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
        var teams = @event.Title?.Split(" vs. "); 
        if (teams?.Length != 2) 
            return;
        
        var home = @event.Teams.FirstOrDefault()?.Name ?? teams[0];
        var away = @event.Teams.LastOrDefault()?.Name ?? teams[1];
                       
        // Do not consider events where the game is already started
        if (@event.StartTime < DateTime.Now)
            return;
        
        // Create registry item
        var registryItem = new RegistryItem
        {
            Id = MatchId.New,
            HomeTeam = home,
            AwayTeam = away,
            StartTime = @event.StartTime,
            PolymarketEvent = @event
        };
        
        // Add registry item
        _events.TryAdd(registryItem.Id, registryItem);
    }

    public async Task TryAttachOddsApi(Odds odds)
    {
        var uncorrelatedItems = _events.Values.Where(e => e.OddsApiEvent == null);
        foreach (var item in uncorrelatedItems)
        {
            var attached = item.TryAttachOddsApiEvent(teamMatcher, odds);
            if (!attached) continue;
            
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
                                  item.OddsApiEvent?.HomeTeam,
                                  item.OddsApiEvent?.AwayTeam,
                                  item.CorrelationScore);
            
            // Create an event actor
            await eventBus.PublishAsync(new RegistryItemCorrelatedEvent {Item =  item});
            return;
        }
    }

    public void Remove(MatchId id)
    {
        _events.TryRemove(id, out _);
        logger.LogInformation("Removed event {Id}. from registry", id);
    }

    public void Reset()
    {
        _events.Clear();
        logger.LogInformation("Cleared event registry");
    }
}
