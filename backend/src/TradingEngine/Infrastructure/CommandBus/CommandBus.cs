using System.Threading.Channels;

namespace TradingEngine.Infrastructure.CommandBus;

/// <summary>
/// Channel-backed implementation of <see cref="ICommandBus"/>.
/// </summary>
public class CommandBus : ICommandBus
{
    // Bounded channel provides back-pressure when producers outpace consumers.
    private readonly Channel<ICommand> _channel = Channel.CreateBounded<ICommand>(new BoundedChannelOptions(10000)
    {
        SingleReader = true, 
        SingleWriter = true
    });

    /// <summary>
    /// Writes a command to the internal channel for asynchronous processing.
    /// </summary>
    /// <param name="command">The command to enqueue.</param>
    /// <param name="cancellationToken">A cancellation token to cancel enqueueing.</param>
    /// <returns>A value task that completes when the command is written.</returns>
    public ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default) 
        => _channel.Writer.WriteAsync(command, cancellationToken);

    /// <summary>
    /// Gets the read side of the command channel used by background workers.
    /// </summary>
    public ChannelReader<ICommand> Reader => _channel.Reader;
}