namespace TradingEngine.Infrastructure.Dispatcher;

// Interface definition for a handler that processes dispatchable events.
// It is generic and works with a specific type of event that implements the IDispatchableEvent interface.
public interface IDispatchableEventHandler<in TEvent> where TEvent : IDispatchableEvent
{
    // Asynchronous method to handle the specified event.
    // Parameters:
    // - @event: The event to be handled. Must implement the IDispatchableEvent interface.
    // - cancellationToken: Optional token to signal cancellation of the operation. Defaults to CancellationToken.None.
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}