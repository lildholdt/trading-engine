using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.CreateMatch;

/// <summary>
/// Event published when a new sport event actor has been created.
/// </summary>
public class MatchCreatedEvent : IEvent
{
    /// <summary>
    /// Gets the initial state snapshot of the newly created actor.
    /// </summary>
    public required MatchState State { get; init; }
}
