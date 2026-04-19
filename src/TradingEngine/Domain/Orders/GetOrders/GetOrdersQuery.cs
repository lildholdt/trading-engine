using TradingEngine.Domain.Matches;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Orders.GetOrders;

/// <summary>
/// Query used to retrieve all orders for a single match.
/// </summary>
public class GetOrdersQuery : IQuery<IReadOnlyCollection<OrderReadModel>>
{
    /// <summary>
    /// Gets the identifier of the match whose orders should be returned.
    /// </summary>
    public required MatchId MatchId { get; init; }
}
