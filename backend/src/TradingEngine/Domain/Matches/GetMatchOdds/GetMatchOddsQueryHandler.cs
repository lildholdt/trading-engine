using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.GetMatchOdds;

/// <summary>
/// Handles <see cref="GetMatchOddsQuery"/> requests by returning odds history for a match.
/// </summary>
public class GetMatchOddsQueryHandler(IMatchReadRepository repository)
    : IQueryHandler<GetMatchOddsQuery, IReadOnlyCollection<OddsReadModel>>
{
    /// <summary>
    /// Retrieves odds read models for the requested match identifier.
    /// </summary>
    /// <param name="query">The query containing the target match identifier.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A collection of odds read models for the match.</returns>
    public async Task<IReadOnlyCollection<OddsReadModel>> HandleAsync(GetMatchOddsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetOddsAsync(query.MatchId, cancellationToken);
    }
}
