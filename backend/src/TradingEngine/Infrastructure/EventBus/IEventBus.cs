namespace TradingEngine.Infrastructure.EventBus;

/// <summary>
/// Defines asynchronous publish operations for domain and integration events.
/// </summary>
public interface IEventBus
{   
    /// <summary>
    /// Publishes an event to the bus so registered handlers can process it.
    /// </summary>
    /// <param name="command">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to cancel enqueueing.</param>
    /// <returns>A value task that completes when the event is written to the bus channel.</returns>
    public ValueTask PublishAsync(IEvent command, CancellationToken cancellationToken = default);
}