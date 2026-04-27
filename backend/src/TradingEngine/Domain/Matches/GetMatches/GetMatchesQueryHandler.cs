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
        var matches = await repository.GetAllAsync(cancellationToken);
        IEnumerable<MatchReadModel> filtered = matches;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            filtered = filtered.Where(m =>
                m.Home.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                m.Away.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (query.StartTimeFromUtc.HasValue)
        {
            filtered = filtered.Where(m => m.StartTime >= query.StartTimeFromUtc.Value);
        }

        if (query.StartTimeToUtc.HasValue)
        {
            filtered = filtered.Where(m => m.StartTime <= query.StartTimeToUtc.Value);
        }

        var sortDesc = string.Equals(query.SortByStartTime, "desc", StringComparison.OrdinalIgnoreCase);
        filtered = sortDesc
            ? filtered.OrderByDescending(m => m.StartTime)
            : filtered.OrderBy(m => m.StartTime);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 50 : query.PageSize;

        return filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}