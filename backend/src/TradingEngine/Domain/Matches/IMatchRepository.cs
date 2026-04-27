namespace TradingEngine.Domain.Matches;

public interface IMatchRepository
{
    Task SaveAsync(Match match, CancellationToken cancellationToken = default);
    Task<Match?> GetById(MatchId id, CancellationToken cancellationToken = default);
    Task RemoveAsync(MatchId id, CancellationToken cancellationToken = default);
}

public interface IMatchReadRepository
{
    Task<IReadOnlyCollection<MatchReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OddsReadModel>> GetOddsAsync(MatchId id, CancellationToken cancellationToken = default);
}