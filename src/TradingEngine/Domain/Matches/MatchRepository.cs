using System.Collections.Concurrent;
using TradingEngine.Domain.Matches.UpdateOdds;

namespace TradingEngine.Domain.Matches;

public record OddsReadModel(string Name, decimal Home, decimal Away, decimal Draw, DateTime UpdatedAt);
public record MatchReadModel(Guid Id, string Home, string Away, DateTime StartTime, IReadOnlyCollection<OddsReadModel> Odds);

public class MatchRepository : IMatchRepository, IOddsRepository, IMatchReadRepository
{
    private readonly ConcurrentDictionary<MatchId, Match> _matches = new();
    private readonly ConcurrentDictionary<MatchId, ConcurrentQueue<IReadOnlyCollection<Bookmaker>>> _odds = new();

    public Task SaveAsync(Match match, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _matches.ContainsKey(match.Id);
        _matches[match.Id] = match;

        // Build odds history over time by appending each new odds snapshot.
        var history = _odds.GetOrAdd(match.Id, _ => new ConcurrentQueue<IReadOnlyCollection<Bookmaker>>());
        if (exists || match.Odds.Count > 0)
        {
            history.Enqueue([.. match.Odds]);
        }

        return Task.CompletedTask;
    }

    public Task<Match?> GetById(MatchId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _matches.TryGetValue(id, out var match);
        return Task.FromResult(match);
    }

    public Task<IReadOnlyCollection<MatchReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var models = _matches.Values
            .Select(match => new MatchReadModel(
                match.Id.Value,
                match.HomeTeam,
                match.AwayTeam,
                match.StartTime,
                _odds.TryGetValue(match.Id, out var history)
                    ? history
                        .SelectMany(snapshot => snapshot.Select(bookmaker => new OddsReadModel(
                            bookmaker.Name,
                            bookmaker.Outcome.Home,
                            bookmaker.Outcome.Away,
                            bookmaker.Outcome.Draw,
                            bookmaker.UpdatedAt)))
                        .ToList()
                    : []))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<MatchReadModel>>(models);
    }
    
    public Task<IReadOnlyCollection<OddsReadModel>> GetOddsAsync(MatchId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_odds.TryGetValue(id, out var history))
        {
            return Task.FromResult<IReadOnlyCollection<OddsReadModel>>([]);
        }

        var models = history
            .SelectMany(snapshot => snapshot.Select(bookmaker => new OddsReadModel(
                bookmaker.Name,
                bookmaker.Outcome.Home,
                bookmaker.Outcome.Away,
                bookmaker.Outcome.Draw,
                bookmaker.UpdatedAt)))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<OddsReadModel>>(models);
    }

    public void Append(OddsUpdatedEvent odds)
    {
        var match = odds.Match;
        var history = _odds.GetOrAdd(match.Id, _ => new ConcurrentQueue<IReadOnlyCollection<Bookmaker>>());
        history.Enqueue([.. match.Odds]);
    }
}