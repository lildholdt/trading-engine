using System.Collections.Concurrent;
using TradingEngine.Domain.UpdateOdds;

namespace TradingEngine.Domain;

public class OddsRepository : IOddsRepository
{
    private readonly ConcurrentDictionary<MatchId, List<Bookmaker>> _store = new();

    public Task SaveAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.AddOrUpdate(
            @event.Id,
            _ => [.. @event.Odds],
            (_, existing) => { existing.AddRange(@event.Odds); return existing; });
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Bookmaker>> GetAllAsync(MatchId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.TryGetValue(id, out var odds);
        return Task.FromResult<IReadOnlyCollection<Bookmaker>>(odds ?? []);
    }
}