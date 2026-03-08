namespace TradingEngine.Infrastructure.EventBus;

// Defines the contract for an event handler within the trading engine infrastructure.
// The event handler processes events of type TEvent, where TEvent implements the IEvent interface.
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    // Handles the event of type TEvent asynchronously.
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}