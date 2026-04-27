namespace TradingEngine.Infrastructure.EventBus;

/// <summary>
/// Handles events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The event type handled by this handler.</typeparam>
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Processes the given event.
    /// </summary>
    /// <param name="event">The event instance to process.</param>
    /// <param name="cancellationToken">A cancellation token to cancel handling.</param>
    /// <returns>A task that completes when handling has finished.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}