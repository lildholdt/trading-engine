using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.GetMatches;

/// <summary>
/// Query used to retrieve all match read models.
/// </summary>
public class GetMatchesQuery : IQuery<IReadOnlyCollection<MatchReadModel>>
{
	/// <summary>
	/// Optional free-text search applied to home and away team names.
	/// </summary>
	public string? Search { get; init; }

	/// <summary>
	/// Optional lower bound filter for match start time (inclusive, UTC).
	/// </summary>
	public DateTime? StartTimeFromUtc { get; init; }

	/// <summary>
	/// Optional upper bound filter for match start time (inclusive, UTC).
	/// </summary>
	public DateTime? StartTimeToUtc { get; init; }

	/// <summary>
	/// Sort direction for start time: <c>asc</c> or <c>desc</c>. Defaults to <c>asc</c>.
	/// </summary>
	public string? SortByStartTime { get; init; }

	/// <summary>
	/// 1-based page number. Defaults to 1.
	/// </summary>
	public int Page { get; init; } = 1;

	/// <summary>
	/// Page size. Defaults to 50.
	/// </summary>
	public int PageSize { get; init; } = 50;
}