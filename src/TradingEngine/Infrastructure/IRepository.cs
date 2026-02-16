namespace TradingEngine.Infrastructure;

// Generic repository interface for managing data persistence operations.
public interface IRepository<TEntity, in TId> 
    where TEntity : IEntity<TId> // Constraint: TEntity must implement IEntity with the same ID type (TId).
    where TId : notnull          // Constraint: TId must be a non-nullable type.
{
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The entity with the specified ID, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds entities that match a given predicate.
    /// </summary>
    /// <param name="predicate">A function to filter the entities based on a condition.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>An enumerable collection of entities that satisfy the predicate.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously saves (adds or updates) an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes an entity from the repository using its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}