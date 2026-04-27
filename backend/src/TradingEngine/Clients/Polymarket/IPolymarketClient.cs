using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// Defines methods for retrieving and streaming Polymarket data.
/// </summary>
public interface IPolymarketClient
{
    /// <summary>
    /// Retrieves available sports entries from Polymarket.
    /// </summary>
    /// <returns>A collection of sport entries.</returns>
    Task<IEnumerable<SportEntry>> GetSports();

    /// <summary>
    /// Retrieves events for a specific Polymarket series.
    /// </summary>
    /// <param name="series">The series to query.</param>
    /// <returns>A collection of events in the specified series.</returns>
    Task<IEnumerable<Event>> GetEvents(PolymarketSeries series);
    
    /// <summary>
    /// Fetches the details of a specific event.
    /// </summary>
    /// <param name="eventId">The unique identifier for the event to retrieve.</param>
    /// <returns>The event details, or <c>null</c> if no event was found.</returns>
    Task<Event?> GetEvent(string eventId);

    /// <summary>
    /// Streams events for a specific series and invokes the callback for each event.
    /// </summary>
    /// <param name="seriesId">The series identifier to stream.</param>
    /// <param name="action">Callback executed for each streamed event.</param>
    Task StreamEvents(string seriesId, Action<Event> action);
}