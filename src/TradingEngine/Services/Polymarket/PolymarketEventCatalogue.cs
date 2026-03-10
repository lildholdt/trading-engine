using System.Collections.Concurrent;
using TradingEngine.Clients.PolyMarket.Models;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Services.PolyMarket;

public class PolymarketEventCatalogue(IEventBus eventBus) : IPolymarketEventCatalogue
{
    private readonly ConcurrentDictionary<string, Event> _store = new();

    public async Task Add(Event entry)
    {
        var added = _store.TryAdd(entry.Id, entry);
        if (added)
        {
            var @event = new PolymarketEventCatalogueEntryAdded { Event = entry };
            await eventBus.PublishAsync(@event);
        }
    }
    
    public IEnumerable<Event> GetAllAsync() 
        => _store.Values.ToList();
}