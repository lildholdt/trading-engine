using TradingEngine.Domain.Matches;

namespace TradingEngine.Domain.Orders;

public interface IOrdersRepository
{
    void SaveOrder(OrderReadModel order);
    IReadOnlyCollection<OrderReadModel> GetOrders(MatchId matchId);
}