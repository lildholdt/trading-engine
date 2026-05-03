using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.PauseMatch;

/// <summary>
/// Handles <see cref="PauseMatchCommand"/> by pausing a specific match actor.
/// </summary>
public class PauseMatchCommandHandler(IMatchActorSystem actorSystem) : ICommandHandler<PauseMatchCommand, Unit>
{
    /// <summary>
    /// Pauses the target match actor and returns <see cref="Unit.Value"/>.
    /// </summary>
    /// <param name="command">The command containing the match identifier to pause.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns><see cref="Unit.Value"/> when pause operation completes.</returns>
    public async Task<Unit> HandleAsync(PauseMatchCommand command, CancellationToken cancellationToken = default)
    {
        await actorSystem.PauseAsync(command.MatchId);
        return Unit.Value;
    }
}
