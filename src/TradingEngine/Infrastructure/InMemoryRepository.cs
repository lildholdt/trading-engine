using System.Collections.Concurrent;

namespace TradingEngine.Infrastructure;

public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : IEntity<TId> // Constraint: TEntity must implement IEntity with the same ID type (TId).
    where TId : notnull          // Constraint: TId must be a non-nullable type.
{
    // Internal storage for entities, implemented as a thread-safe dictionary.
    private readonly ConcurrentDictionary<TId, TEntity> _store = new();

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation.
        _store.TryGetValue(id, out var entity);
        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation.
        return await Task.FromResult(_store.Values.Where(predicate));
    }

    public Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation.
        if (_store.ContainsKey(entity.Id))
        {
            _store[entity.Id] = entity;
        }
        else
        {
            _store.TryAdd(entity.Id, entity);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation.
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}