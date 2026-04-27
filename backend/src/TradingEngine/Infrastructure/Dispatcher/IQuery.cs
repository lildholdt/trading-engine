namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Represents a query that returns a result when handled.
/// </summary>
/// <typeparam name="TResult">The result type returned by the query handler.</typeparam>
public interface IQuery<TResult>;