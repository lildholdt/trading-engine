using TradingEngine.Utils;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.OddsApi;
using TradingEngine.Services.PolyMarket;
using IOddsApiEventCatalogue = TradingEngine.Services.OddsApi.IOddsApiEventCatalogue;

namespace TradingEngine.Services;

public class EventCorrelationService(
    IPolymarketEventCatalogue polymarketEventCatalogue,
    IOddsApiEventCatalogue oddsApiEventCatalogue,
    ITeamMatcher teamMatcher,
    ILogger<EventCorrelationService> logger)
    : IEventHandler<PolymarketEventCatalogueEntryAdded>, IEventHandler<OddsApiEventCatalogueEntryAdded>
{
    public Task HandleAsync(PolymarketEventCatalogueEntryAdded @event, CancellationToken cancellationToken)
    {
        Console.WriteLine(@event);
        return Task.CompletedTask;
    }
    
    public Task HandleAsync(OddsApiEventCatalogueEntryAdded @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{@event.Match.HomeTeam} vs  {@event.Match.AwayTeam}");
        return Task.CompletedTask;
    }
}

