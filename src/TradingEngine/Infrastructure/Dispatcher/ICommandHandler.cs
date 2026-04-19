namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Handles a command and returns a typed result.
/// </summary>
/// <typeparam name="TCommand">The command type handled by this handler.</typeparam>
/// <typeparam name="TResult">The result type returned by handling the command.</typeparam>
public interface ICommandHandler<in TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	/// <summary>
	/// Handles the provided command.
	/// </summary>
	/// <param name="command">The command instance to execute.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>The result of command execution.</returns>
	Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}