namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Represents a dispatcher that is responsible for queueing and dispatching events to their respective handlers.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Enqueues an event for asynchronous dispatch to all registered handlers.
    /// </summary>
    /// <param name="event">The event to be dispatched. It must implement the <see cref="IDispatchableEvent"/> interface.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the enqueue operation.</param>
    void Enqueue(IDispatchableEvent @event, CancellationToken cancellationToken = default);
}