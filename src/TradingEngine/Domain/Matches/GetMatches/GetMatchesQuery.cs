using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.GetMatches;

/// <summary>
/// Query used to retrieve all match read models.
/// </summary>
public class GetMatchesQuery : IQuery<IReadOnlyCollection<MatchReadModel>>;