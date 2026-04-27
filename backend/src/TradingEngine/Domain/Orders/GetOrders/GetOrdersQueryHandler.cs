using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Orders.GetOrders;

/// <summary>
/// Handles <see cref="GetOrdersQuery"/> requests by returning stored orders for a match.
/// </summary>
public class GetOrdersQueryHandler(IOrdersRepository ordersRepository)
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderReadModel>>
{
    /// <summary>
    /// Retrieves order read models for the requested match identifier.
    /// </summary>
    /// <param name="query">The query containing the target match identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of order read models for the match.</returns>
    public Task<IReadOnlyCollection<OrderReadModel>> HandleAsync(GetOrdersQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ordersRepository.GetOrders(query.MatchId));
    }
}
