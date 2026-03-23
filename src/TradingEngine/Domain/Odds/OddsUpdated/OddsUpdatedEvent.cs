using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Odds.OddsUpdated;

public class OddsUpdatedEvent : IEvent
{
    public required SportEventId Id { get; init; }
    public required IReadOnlyCollection<Bookmaker> Odds { get; init; }
}