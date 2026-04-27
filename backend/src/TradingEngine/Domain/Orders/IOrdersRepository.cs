using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Orders.GetOrders;

namespace TradingEngine.Domain.Orders;

public interface IOrdersRepository
{
    void SaveOrder(OrderReadModel order);
    IReadOnlyCollection<OrderReadModel> GetOrders(MatchId matchId);
}