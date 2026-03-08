using System.Threading.Channels;

namespace TradingEngine.Infrastructure.EventBus;

public class EventBus : IEventBus
{
    private readonly Channel<IEvent> _channel = Channel.CreateBounded<IEvent>(new BoundedChannelOptions(10000)
    {
        SingleReader = true, 
        SingleWriter = true
    });

    public ValueTask PublishAsync(IEvent @event, CancellationToken cancellationToken = default) 
        => _channel.Writer.WriteAsync(@event, cancellationToken);
    
    public ChannelReader<IEvent> Reader => _channel.Reader;
}