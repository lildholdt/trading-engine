using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.StopMatch;

/// <summary>
/// Command used to stop a specific active match.
/// </summary>
public class StopMatchCommand : ICommand<Unit>
{
    /// <summary>
    /// Gets the identifier of the match to stop.
    /// </summary>
    public required MatchId MatchId { get; init; }
}
