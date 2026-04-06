using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Domain;

/// <summary>
/// Defines the contract for a provider responsible for retrieving odds associated with a specific sport event.
/// </summary>
public interface IOddsProvider
{
    /// <summary>
    /// Retrieves the odds for a given sport event based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event.</param>
    /// <returns>
    /// A task that resolves to an <see cref="Outcome"/> object representing the odds for the specified sport event,
    /// or <c>null</c> if no odds are available for the provided event ID.
    /// </returns>
    public Task<IReadOnlyCollection<Bookmaker>> GetOdds(SportEventId id);
}