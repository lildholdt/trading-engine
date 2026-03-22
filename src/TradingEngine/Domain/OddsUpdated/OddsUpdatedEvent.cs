using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.OddsUpdated;

public class OddsUpdatedEvent : IEvent
{
    public required SportEventId Id { get; init; }
    public required IReadOnlyCollection<Bookmaker> Bookmakers { get; init; }
}