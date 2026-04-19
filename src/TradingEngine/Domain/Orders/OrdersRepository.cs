using System.Collections.Concurrent;
using TradingEngine.Domain.Matches;

namespace TradingEngine.Domain.Orders;

public class OrdersRepository : IOrdersRepository
{
    private readonly ConcurrentDictionary<MatchId, ConcurrentQueue<OrderReadModel>> _orders = new();
    
    public void SaveOrder(OrderReadModel order)
    {
        var matchId = (MatchId)order.Id;
        var queue = _orders.GetOrAdd(matchId, _ => new ConcurrentQueue<OrderReadModel>());
        queue.Enqueue(order);
    }

    public IReadOnlyCollection<OrderReadModel> GetOrders(MatchId matchId)
    {
        if (_orders.TryGetValue(matchId, out var orders))
        {
            return orders.ToArray();
        }

        return [];
    }
}