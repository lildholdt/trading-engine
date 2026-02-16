namespace TradingEngine.Infrastructure;

// Generic repository interface for managing data persistence.
public interface IRepository<TEntity, in TId> 
    where TEntity : IEntity<TId> // Constraint: TEntity must implement IEntity with the same ID type (TId).
    where TId : notnull          // Constraint: TId must be a non-nullable type.
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity with the specified ID or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TId id);

    /// <summary>
    /// Finds entities that match a given predicate.
    /// </summary>
    /// <param name="predicate">A function to filter the entities.</param>
    /// <returns>An enumerable collection of entities that match the predicate.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate);

    /// <summary>
    /// Saves (adds or updates) an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    Task SaveAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    Task DeleteAsync(TId id);
}