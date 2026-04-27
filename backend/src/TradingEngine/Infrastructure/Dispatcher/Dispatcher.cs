namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Default in-process dispatcher that resolves handlers from dependency injection.
/// </summary>
/// <param name="serviceProvider">Service provider used to resolve command and query handlers.</param>
public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    /// <summary>
    /// Resolves and invokes the matching command handler.
    /// </summary>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="command">The command instance to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The command handler result.</returns>
    public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);
        return await ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);
    }

    /// <summary>
    /// Resolves and invokes the matching query handler.
    /// </summary>
    /// <typeparam name="TResult">The query result type.</typeparam>
    /// <param name="query">The query instance to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The query handler result.</returns>
    public async Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = serviceProvider.GetRequiredService(handlerType);
        return await ((dynamic)handler).HandleAsync((dynamic)query, cancellationToken);
    }
}