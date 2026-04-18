namespace TradingEngine.Domain.Matches;

public interface IMatchRepository
{
    Task SaveAsync(Match match, CancellationToken cancellationToken = default);
    Task<Match?> GetById(MatchId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Match>> GetAllAsync(CancellationToken cancellationToken = default);
}