using TradingEngine.Domain.UpdateOdds;

namespace TradingEngine.Domain;

public interface IOddsRepository
{
    Task SaveAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Bookmaker>> GetAllAsync(MatchId id, CancellationToken cancellationToken = default);
}