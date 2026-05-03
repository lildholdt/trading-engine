using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches.CreateMatch;

/// <summary>
/// Event published when a new sport event actor has been created.
/// </summary>
public class MatchCreatedEvent : IEvent
{
    /// <summary>
    /// Gets the initial state snapshot of the newly created actor.
    /// </summary>
    public required MatchId MatchId { get; init; }

    /// <summary>
    /// Gets the home team name.
    /// </summary>
    public required string HomeTeam { get; init; }

    /// <summary>
    /// Gets the away team name.
    /// </summary>
    public required string AwayTeam { get; init; }

    /// <summary>
    /// Gets the competition or series name.
    /// </summary>
    public required string Series { get; init; }

    /// <summary>
    /// Gets the scheduled match start time.
    /// </summary>
    public required DateTime StartTime { get; init; }

    /// <summary>
    /// Gets when the match actor was created.
    /// </summary>
    public required DateTime CreatedAtUtc { get; init; }
}
