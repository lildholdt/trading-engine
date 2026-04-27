using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.GetMatchOdds;

/// <summary>
/// Query used to retrieve odds history for a single match.
/// </summary>
public class GetMatchOddsQuery : IQuery<IReadOnlyCollection<OddsReadModel>>
{
    /// <summary>
    /// Gets the identifier of the match whose odds should be returned.
    /// </summary>
    public required MatchId MatchId { get; init; }
}
