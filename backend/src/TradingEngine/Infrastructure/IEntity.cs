namespace TradingEngine.Infrastructure;

// Defines the contract for an entity within the trading engine infrastructure.
// Represents an object that has a unique identifier of type TId.
public interface IEntity<TId> where TId : notnull
{
    // Gets the unique identifier of the entity.
    // The identifier is immutable, meaning it can only be set during initialization.
    TId Id { get; init; }
}