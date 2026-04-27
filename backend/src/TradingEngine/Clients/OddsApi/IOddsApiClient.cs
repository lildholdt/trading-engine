using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Clients.OddsApi;

/// <summary>
/// Defines methods for retrieving sports odds data from the Odds API.
/// </summary>
public interface IOddsApiClient
{
    /// <summary>
    /// Retrieves all available odds for the specified sport type.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <returns>A read-only collection of odds entries.</returns>
    Task<IReadOnlyCollection<Odds>> GetOdds(OddsApiSportsType oddsApiSportsType);

    /// <summary>
    /// Retrieves odds for a specific event within the specified sport type.
    /// </summary>
    /// <param name="oddsApiSportsType">The sport type to query.</param>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The matching odds entry when found; otherwise <c>null</c>.</returns>
    Task<Odds?> GetOdds(OddsApiSportsType oddsApiSportsType, string eventId);
}