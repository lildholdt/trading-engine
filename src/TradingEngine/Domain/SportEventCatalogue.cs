using TradingEngine.Domain.SportEventCatalogueEntryAdded;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public interface ISportEventCatalogue
{
    Task SaveAsync(SportEventCatalogueEntry entry, CancellationToken cancellationToken = default);
    Task<IEnumerable<SportEventCatalogueEntry>> GetAllAsync(CancellationToken cancellationToken = default);

}

public class SportEventCatalogue(
    IRepository<SportEventCatalogueEntry, string> repository,
    IEventBus eventBus) : ISportEventCatalogue
{
    public async Task SaveAsync(SportEventCatalogueEntry entry, CancellationToken cancellationToken = default)
    {
        // Save to the repository
        await repository.SaveAsync(entry, cancellationToken);
        
        // Publish sport event catalogue entry added event 
        await eventBus.PublishAsync(new SportEventCatalogueEntryAddedEvent
        {
            SportEvent = entry
        }, cancellationToken);
    }

        public async Task<IEnumerable<SportEventCatalogueEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}