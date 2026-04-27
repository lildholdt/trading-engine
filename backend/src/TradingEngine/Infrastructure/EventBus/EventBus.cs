using System.Threading.Channels;

namespace TradingEngine.Infrastructure.EventBus;

/// <summary>
/// Channel-backed event bus implementation.
/// </summary>
public class EventBus : IEventBus
{
    // Bounded channel provides back-pressure when event producers are faster than consumers.
    private readonly Channel<IEvent> _channel = Channel.CreateBounded<IEvent>(new BoundedChannelOptions(10000)
    {
        SingleReader = true, 
        SingleWriter = true
    });

    /// <summary>
    /// Publishes an event to the internal channel.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to cancel enqueueing.</param>
    /// <returns>A value task that completes when the event is written.</returns>
    public ValueTask PublishAsync(IEvent @event, CancellationToken cancellationToken = default) 
        => _channel.Writer.WriteAsync(@event, cancellationToken);
    
    /// <summary>
    /// Gets the read side of the event channel used by background workers.
    /// </summary>
    public ChannelReader<IEvent> Reader => _channel.Reader;
}