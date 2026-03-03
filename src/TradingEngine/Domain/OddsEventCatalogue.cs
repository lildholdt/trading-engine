using System.Collections.Concurrent;
using TradingEngine.Domain;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;


public interface IOddsEventCatalogue
{
    Task SaveAsync(OddsEventCatalogueEntry entry, CancellationToken cancellationToken = default);
    Task<IEnumerable<OddsEventCatalogueEntry>> GetAllAsync(CancellationToken cancellationToken = default);
}
public class OddsEventCatalogue(
    IRepository<OddsEventCatalogueEntry, string> repository,
    IEventBus eventBus) : IOddsEventCatalogue
{

    public async Task SaveAsync(OddsEventCatalogueEntry entry, CancellationToken cancellationToken = default)
    {
        // Save to the repository
        await repository.SaveAsync(entry, cancellationToken);
        
        // Publish sport event catalogue entry added event 
        await eventBus.PublishAsync(new OddsEventCatalogueEntryAddedEvent
        {
            OddsEvent = entry
        }, cancellationToken);
    }

    public async Task<IEnumerable<OddsEventCatalogueEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}
