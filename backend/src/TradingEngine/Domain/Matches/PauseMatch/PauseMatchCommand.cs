using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.PauseMatch;

/// <summary>
/// Command used to pause odds polling for an active match actor.
/// </summary>
public class PauseMatchCommand : ICommand<Unit>
{
    /// <summary>
    /// Gets the identifier of the match to pause.
    /// </summary>
    public required MatchId MatchId { get; init; }
}
