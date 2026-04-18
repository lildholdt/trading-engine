using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.ActivateMatch;

/// <summary>
/// Event published when a sport event actor has started processing.
/// </summary>
public class MatchActivated : IEvent
{
    /// <summary>
    /// Gets the identifier of the actor that started.
    /// </summary>
    public required MatchId Id { get; init; }
}
