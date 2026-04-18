using System.Collections.Concurrent;

namespace TradingEngine.Domain.Matches;

public class MatchRepository : IMatchRepository
{
    private readonly ConcurrentDictionary<MatchId, Match> _matches = new();

    public Task SaveAsync(Match match, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _matches[match.Id] = match;
        return Task.CompletedTask;
    }

    public Task<Match?> GetById(MatchId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _matches.TryGetValue(id, out var match);
        return Task.FromResult(match);
    }

    public Task<IReadOnlyCollection<Match>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyCollection<Match>>(_matches.Values.ToList());
    }
}