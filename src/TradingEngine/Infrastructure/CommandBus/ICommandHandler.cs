namespace TradingEngine.Infrastructure.CommandBus;

/// <summary>
/// Handles a command of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The command type this handler processes.</typeparam>
public interface ICommandHandler<in T> where T : ICommand
{
    /// <summary>
    /// Executes the command handling logic.
    /// </summary>
    /// <param name="command">The command instance to process.</param>
    /// <param name="cancellationToken">A cancellation token to cancel processing.</param>
    /// <returns>A task that completes when handling finishes.</returns>
    Task HandleAsync(T command, CancellationToken cancellationToken = default);
}