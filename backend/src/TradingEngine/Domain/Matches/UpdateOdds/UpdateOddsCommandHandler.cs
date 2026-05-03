using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.UpdateOdds;

/// <summary>
/// Handles <see cref="UpdateOddsCommand"/> by updating odds for a specific match actor.
/// </summary>
public class UpdateOddsCommandHandler(IMatchActorSystem actorSystem) : ICommandHandler<UpdateOddsCommand, Unit>
{
    /// <summary>
    /// Updates odds for the target match actor and returns <see cref="Unit.Value"/>.
    /// </summary>
    /// <param name="command">The command containing the match identifier and new odds.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns><see cref="Unit.Value"/> when the odds update operation completes.</returns>
    public async Task<Unit> HandleAsync(UpdateOddsCommand command, CancellationToken cancellationToken = default)
    {
        await actorSystem.SendAsync(command);
        return Unit.Value;
    }
}
