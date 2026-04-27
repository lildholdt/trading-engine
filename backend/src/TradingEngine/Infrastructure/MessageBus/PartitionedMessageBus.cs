using System.Collections.Concurrent;
using System.Threading.Channels;

namespace TradingEngine.Infrastructure.MessageBus;

public abstract class PartitionedMessageBus<TMessage> : IMessageBus<TMessage> where TMessage : IPartitionedMessage
{
    private readonly ConcurrentDictionary<string, Channel<TMessage>> _channels = new();

    protected Channel<TMessage> GetChannel(string partitionKey)
        => _channels.GetOrAdd(partitionKey, _ => Channel.CreateUnbounded<TMessage>());
    
    public ValueTask SendAsync(TMessage message, CancellationToken cancellationToken)
    {
        var channel = GetChannel(message.PartitionKey);
        return  channel.Writer.WriteAsync(message, cancellationToken);
    }
}