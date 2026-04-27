using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.GetRegistryItems;

public class GetRegistryItemsQueryHandler(IRegistry registry)
    : IQueryHandler<GetRegistryItemsQuery, IReadOnlyCollection<RegistryItemReadModel>>
{
    public Task<IReadOnlyCollection<RegistryItemReadModel>> HandleAsync(GetRegistryItemsQuery query,
        CancellationToken cancellationToken = default)
    {
        var models = registry.GetAll().Select(x =>
            new RegistryItemReadModel(
                x.Id.Value,
                x.HomeTeam,
                x.AwayTeam,
                x.OddsApiEvent?.HomeTeam,
                x.OddsApiEvent?.AwayTeam,
                x.CorrelationScore));

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            models = models.Where(x =>
                x.PolymarketHome.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.PolymarketAway.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (x.OddsApiHome != null && x.OddsApiHome.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (x.OddsApiAway != null && x.OddsApiAway.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        var sortBy = query.SortBy?.Trim().ToLowerInvariant();
        var desc = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        models = sortBy switch
        {
            "away" => desc
                ? models.OrderByDescending(x => x.PolymarketAway)
                : models.OrderBy(x => x.PolymarketAway),
            "correlation" => desc
                ? models.OrderByDescending(x => x.CorrelationScore)
                : models.OrderBy(x => x.CorrelationScore),
            _ => desc
                ? models.OrderByDescending(x => x.PolymarketHome)
                : models.OrderBy(x => x.PolymarketHome)
        };

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 50 : query.PageSize;

        var paged = models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<RegistryItemReadModel>>(paged);
    }
}
