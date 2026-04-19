using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.Reset;

/// <summary>
/// Handles <see cref="ResetMatchesCommand"/> by resetting the match actor system.
/// </summary>
public class ResetMatchesCommandHandler(IMatchActorSystem actorSystem) : ICommandHandler<ResetMatchesCommand, Unit>
{
    /// <summary>
    /// Resets all active match actors and returns <see cref="Unit.Value"/>.
    /// </summary>
    /// <param name="command">The reset command payload.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns><see cref="Unit.Value"/> when reset operation completes.</returns>
    public async Task<Unit> HandleAsync(ResetMatchesCommand command, CancellationToken cancellationToken = default)
    {
        await actorSystem.Reset();
        return Unit.Value;
    }
}
