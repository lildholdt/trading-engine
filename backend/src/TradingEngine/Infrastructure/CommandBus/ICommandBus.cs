namespace TradingEngine.Infrastructure.CommandBus;

/// <summary>
/// Defines asynchronous command dispatch operations.
/// </summary>
public interface ICommandBus
{   
    /// <summary>
    /// Enqueues a command for asynchronous processing.
    /// </summary>
    /// <param name="command">The command to enqueue.</param>
    /// <param name="cancellationToken">A cancellation token to cancel enqueueing.</param>
    /// <returns>A value task that completes when the command is written to the bus channel.</returns>
    public ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default);
}