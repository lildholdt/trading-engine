namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Handles a query and returns a typed result.
/// </summary>
/// <typeparam name="TQuery">The query type handled by this handler.</typeparam>
/// <typeparam name="TResult">The result type returned by handling the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
	where TQuery : IQuery<TResult>
{
	/// <summary>
	/// Handles the provided query.
	/// </summary>
	/// <param name="query">The query instance to execute.</param>
	/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
	/// <returns>The query result.</returns>
	Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}