using System.Threading.Channels;

namespace TradingEngine.Infrastructure.MessageBus;

public class ChannelMessageBus<TMessage> : IMessageBus<TMessage> where TMessage : IMessage
{
    private readonly Channel<TMessage> _channel;

    protected ChannelMessageBus(int capacity = 10000)
    {
        _channel = Channel.CreateBounded<TMessage>(capacity);
    }
    
    public ValueTask SendAsync(TMessage message, CancellationToken cancellationToken)
        => _channel.Writer.WriteAsync(message, cancellationToken);
    
    public ChannelReader<TMessage> Reader => _channel.Reader;
}