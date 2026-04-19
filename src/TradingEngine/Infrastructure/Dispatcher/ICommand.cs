namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Represents a command that produces a result when handled.
/// </summary>
/// <typeparam name="TResult">The result type returned by the command handler.</typeparam>
public interface ICommand<TResult>;