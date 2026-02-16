namespace TradingEngine.Infrastructure.EventBus;

// Defines the contract for an event bus in the trading engine infrastructure.
public interface IEventBus
{
    // Publishes an event of type TEvent asynchronously. 
    // TEvent must be a reference type (class).
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;

    // Subscribes to an event of type TEvent with a specified action or handler.
    // TEvent must be a reference type (class).
    void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;
}