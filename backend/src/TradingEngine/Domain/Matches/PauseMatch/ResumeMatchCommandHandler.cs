using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.PauseMatch;

/// <summary>
/// Handles <see cref="ResumeMatchCommand"/> by resuming a paused match actor.
/// </summary>
public class ResumeMatchCommandHandler(IMatchActorSystem actorSystem) : ICommandHandler<ResumeMatchCommand, Unit>
{
    /// <summary>
    /// Resumes the target match actor and returns <see cref="Unit.Value"/>.
    /// </summary>
    /// <param name="command">The command containing the match identifier to resume.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns><see cref="Unit.Value"/> when resume operation completes.</returns>
    public async Task<Unit> HandleAsync(ResumeMatchCommand command, CancellationToken cancellationToken = default)
    {
        await actorSystem.ResumeAsync(command.MatchId);
        return Unit.Value;
    }
}
