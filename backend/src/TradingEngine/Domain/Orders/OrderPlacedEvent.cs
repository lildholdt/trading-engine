using TradingEngine.Domain.Matches;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Orders;

public class OrderPlacedEvent : IEvent
{
    public required Match Match { get; init; }
}