using TradingEngine.Domain.Matches;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Orders;

public class OrderPlacedEvent : IEvent
{
    public required MatchId MatchId { get; init; }
    public IReadOnlyCollection<Bookmaker> Odds { get; init; }
    public decimal? HomePrice { get; init; }
    public decimal? AwayPrice { get; init; }
    public decimal? DrawPrice { get; init; }
    public decimal AverageHomeOdds { get; init; }
    public decimal AverageAwayOdds { get; init; }
    public decimal AverageDrawOdds { get; init; }
}