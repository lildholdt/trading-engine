using System.Collections.Concurrent;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Services.OddsApi;

public class OddsApiEventCatalogue(IEventBus eventBus) : IOddsApiEventCatalogue
{
    private readonly ConcurrentDictionary<string, Match> _store = new();

    public async Task Add(Match entry)
    {
        var added = _store.TryAdd(entry.Id, entry);
        if (added)
        {
            var @event = new OddsApiEventCatalogueEntryAdded { Match = entry };
            await eventBus.PublishAsync(@event);
        }
    }

    public IEnumerable<Match> GetAllAsync()
        => _store.Values.ToList();
}