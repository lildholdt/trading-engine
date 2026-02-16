using System.Collections.Concurrent;
using System.Threading.Channels;

namespace TradingEngine.Infrastructure.Dispatcher;

public sealed class Dispatcher(IServiceProvider services) : IDispatcher
{
    private readonly ConcurrentDictionary<string, Channel<IDispatchableEvent>> _partitions = new();

    public void Enqueue(IDispatchableEvent evt, CancellationToken cancellationToken = default)
    {
        var channel = _partitions.GetOrAdd(
            evt.PartitionKey,
            _ => StartPartitionProcessor(evt.PartitionKey)
        );

        channel.Writer.TryWrite(evt);
    }

    private Channel<IDispatchableEvent> StartPartitionProcessor(string key)
    {
        var channel = Channel.CreateUnbounded<IDispatchableEvent>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }
        );

        _ = Task.Run(() => ProcessPartition(key, channel));

        return channel;
    }

    private async Task ProcessPartition(string key, Channel<IDispatchableEvent> channel)
    {
        await foreach (var evt in channel.Reader.ReadAllAsync())
        {
            await Dispatch(evt);
        }
    }

    private async Task Dispatch(IDispatchableEvent evt)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var handlerType = typeof(IDispatchableEventHandler<>).MakeGenericType(evt.GetType());
        var handlers = provider.GetServices(handlerType).ToList();

        // Enforce exactly one handler per event type
        if (handlers.Count == 0)
            throw new InvalidOperationException($"No handler registered for {evt.GetType().Name}");
        if (handlers.Count > 1)
            throw new InvalidOperationException($"Multiple handlers registered for {evt.GetType().Name} - only one allowed");

        var handler = handlers[0];
        var method = handlerType.GetMethod(nameof(IDispatchableEventHandler<>.HandleAsync))!;
        await (Task)method.Invoke(handler, [evt, CancellationToken.None])!;
    }

    
    
    /*
    
    private readonly Channel<IDispatchableEvent> _channel = Channel.CreateBounded<IDispatchableEvent>(
        new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });
    
    private readonly ConcurrentDictionary<Type, List<Action<IDispatchableEvent>>> _handlers = new();

    public void Register<T>(Action<T> handler) where T : IDispatchableEvent
    {
        var eventType = typeof(T);

        _handlers.AddOrUpdate(
            eventType,
            _ => [e => handler((T)e)],
            (_, list) =>
            {
                list.Add(e => handler((T)e));
                return list;
            });
    }

    public ValueTask DispatchAsync(IDispatchableEvent @event, CancellationToken cancellationToken)
        => _channel.Writer.WriteAsync(@event, cancellationToken);

    public async Task StartAsync(CancellationToken ct)
    {
        await foreach (var evt in _channel.Reader.ReadAllAsync(ct))
        {
            if (_handlers.TryGetValue(evt.GetType(), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(evt); // synchronous for determinism
                }
            }
        }
    }
    
    */
}
