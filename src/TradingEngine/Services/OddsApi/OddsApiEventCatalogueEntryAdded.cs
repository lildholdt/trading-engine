using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Services.OddsApi;

public class OddsApiEventCatalogueEntryAdded : IEvent
{
    public required Match Match { get; init; }
}