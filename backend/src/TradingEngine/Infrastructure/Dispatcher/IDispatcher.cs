namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Defines synchronous in-process dispatch operations for commands and queries.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Sends a command to its handler and returns the handler result.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The command handler result.</returns>
    Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query to its handler and returns the query result.
    /// </summary>
    /// <typeparam name="TResult">The expected result type.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The query handler result.</returns>
    Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}