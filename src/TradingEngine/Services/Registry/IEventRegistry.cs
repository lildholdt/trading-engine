using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;

namespace TradingEngine.Services.Registry;

/// <summary>
/// Interface that defines methods for managing and interacting with events in an event registry.
/// </summary>
public interface IEventRegistry
{
    /// <summary>
    /// Retrieves a sport event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event to retrieve.</param>
    public RegistryItem? Get(SportEventId id);

    /// <summary>
    /// Registers a new PolyMarket event into the event registry.
    /// </summary>
    /// <param name="event">The PolyMarket event to register.</param>
    public void RegisterPolymarket(Event @event);

    /// <summary>
    /// Attaches an OddsApi event to the registry, allowing it to be associated with other events.
    /// </summary>
    /// <param name="event">The OddsApi event to attach.</param>
    public void AttachOddsApi(Odds @event);
}