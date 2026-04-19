using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.GetRegistryItems;

public class GetRegistryItemsQuery : IQuery<IReadOnlyCollection<RegistryItemReadModel>>
{
	public string? Search { get; init; }
	public string? SortBy { get; init; }
	public string? SortDirection { get; init; }
	public int Page { get; init; } = 1;
	public int PageSize { get; init; } = 50;
}
