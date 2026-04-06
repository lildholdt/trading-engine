using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;

namespace TradingEngine.Infrastructure.Registry;

/// <summary>
/// Interface that defines methods for managing and interacting with events in an event registry.
/// </summary>
public interface IEventRegistry
{
    /// <summary>
    /// Retrieves a sport event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event to retrieve.</param>
    public EventRegistryItem? Get(SportEventId id);

    /// <summary>
    /// Retrieves all events currently stored in the registry.
    /// </summary>
    /// <returns>A read-only collection of registered events.</returns>
    public IReadOnlyCollection<EventRegistryItem> GetAll();

    /// <summary>
    /// Retrieves the registry synchronization configuration entries.
    /// </summary>
    /// <returns>A read-only collection of configuration items.</returns>
    public IReadOnlyCollection<EventRegistryConfigurationItem> GetConfiguration();

    /// <summary>
    /// Updates the activation state of a configuration entry.
    /// </summary>
    /// <param name="id">The identifier of the configuration entry to update.</param>
    /// <param name="state">The new active state to apply.</param>
    public void UpdateConfiguration(int id, bool state);
    
    /// <summary>
    /// Registers a new PolyMarket event into the event registry.
    /// </summary>
    /// <param name="event">The PolyMarket event to register.</param>
    public void RegisterPolymarket(Event @event);

    /// <summary>
    /// Attaches an OddsApi event to the registry, allowing it to be associated with other events.
    /// </summary>
    /// <param name="event">The OddsApi event to attach.</param>
    public Task AttachOddsApi(Odds @event);
    
    /// <summary>
    /// Removes a sport event from the registry by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the event to remove.</param>
    public void Remove(SportEventId id);

    /// <summary>
    /// Clears all events from the registry.
    /// </summary>
    public void Reset();
}