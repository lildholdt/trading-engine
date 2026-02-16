using System.Collections.Concurrent;

namespace TradingEngine.Infrastructure;

public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, TEntity> _store = new();

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        _store.TryGetValue(id, out var entity);
        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate)
    {
        return await Task.FromResult(_store.Values.Where(predicate));
    }

    public Task SaveAsync(TEntity entity)
    {
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

    public Task DeleteAsync(TId id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}