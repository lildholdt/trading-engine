using TradingEngine.Clients.PolyMarket.Models;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Services.PolyMarket;

public class PolymarketEventCatalogueEntryAdded : IEvent
{
    public required Event Event { get; init; }
}