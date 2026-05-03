using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.ResumeMatch;

/// <summary>
/// Command used to resume odds polling for a paused match actor.
/// </summary>
public class ResumeMatchCommand : ICommand<Unit>
{
    /// <summary>
    /// Gets the identifier of the match to resume.
    /// </summary>
    public required MatchId MatchId { get; init; }
}
