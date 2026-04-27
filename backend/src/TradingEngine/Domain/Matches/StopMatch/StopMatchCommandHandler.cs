using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.StopMatch;

/// <summary>
/// Handles <see cref="StopMatchCommand"/> by stopping a specific match actor.
/// </summary>
public class StopMatchCommandHandler(IMatchActorSystem actorSystem) : ICommandHandler<StopMatchCommand, Unit>
{
    /// <summary>
    /// Stops the target match actor and returns <see cref="Unit.Value"/>.
    /// </summary>
    /// <param name="command">The command containing the match identifier to stop.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns><see cref="Unit.Value"/> when stop operation completes.</returns>
    public async Task<Unit> HandleAsync(StopMatchCommand command, CancellationToken cancellationToken = default)
    {
        await actorSystem.StopAsync(command.MatchId);
        return Unit.Value;
    }
}
