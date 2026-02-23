using System.Threading.Channels;

namespace TradingEngine.Infrastructure.CommandBus;

public class CommandBus : ICommandBus
{
    private readonly Channel<ICommand> _channel = Channel.CreateBounded<ICommand>(new BoundedChannelOptions(10000)
    {
        SingleReader = true, 
        SingleWriter = true
    });

    public ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default) 
        => _channel.Writer.WriteAsync(command, cancellationToken);

    public ChannelReader<ICommand> Reader => _channel.Reader;
}