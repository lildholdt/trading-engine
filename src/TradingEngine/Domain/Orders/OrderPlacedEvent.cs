using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.OrderPlaced;

public class OrderPlacedEvent : IEvent
{
    public MatchId MatchId { get; init; }
    public decimal Home { get; init; }
    public decimal Away { get; init; }
    public decimal Draw { get; init; }
}