using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.GetMatches;

/// <summary>
/// Handles <see cref="GetMatchesQuery"/> requests by returning all match read models.
/// </summary>
public class GetMatchesQueryHandler(IMatchReadRepository repository) : IQueryHandler<GetMatchesQuery, IReadOnlyCollection<MatchReadModel>>
{
    /// <summary>
    /// Retrieves all matches from the read repository.
    /// </summary>
    /// <param name="query">The query payload.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>All available match read models.</returns>
    public async Task<IReadOnlyCollection<MatchReadModel>> HandleAsync(GetMatchesQuery query, CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}