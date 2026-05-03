using System.Collections.Concurrent;
using TradingEngine.Domain.Matches.CreateMatch;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches;

public record OddsReadModel(string Name, decimal Home, decimal Away, decimal Draw, DateTime UpdatedAt);
public record MatchReadModel(
    Guid Id,
    string Home,
    string Away,
    string Series,
    DateTime StartTime,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? LastOddsUpdateAtUtc,
    DateTime? StoppedAtUtc,
    IReadOnlyCollection<OddsReadModel> Odds);

internal sealed class MatchReadState
{
    public required Guid Id { get; init; }
    public required string Home { get; set; }
    public required string Away { get; set; }
    public required string Series { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastOddsUpdateAtUtc { get; set; }
    public DateTime? StoppedAtUtc { get; set; }
    public ConcurrentQueue<IReadOnlyCollection<OddsReadModel>> OddsSnapshots { get; } = new();
}

public class InMemoryMatchReadRepository :
    IMatchReadRepository,
    IEventHandler<MatchCreatedEvent>,
    IEventHandler<OddsUpdatedEvent>,
    IEventHandler<MatchStoppedEvent>
{
    private readonly ConcurrentDictionary<MatchId, MatchReadState> _matches = new();

    public Task<IReadOnlyCollection<MatchReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var models = _matches.Values.Select(ToReadModel).ToList();
        return Task.FromResult<IReadOnlyCollection<MatchReadModel>>(models);
    }

    public Task<IReadOnlyCollection<OddsReadModel>> GetOddsAsync(MatchId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_matches.TryGetValue(id, out var state))
        {
            return Task.FromResult<IReadOnlyCollection<OddsReadModel>>([]);
        }

        var odds = state.OddsSnapshots.SelectMany(snapshot => snapshot).ToList();
        return Task.FromResult<IReadOnlyCollection<OddsReadModel>>(odds);
    }

    public Task HandleAsync(MatchCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _matches.AddOrUpdate(
            @event.MatchId,
            _ => new MatchReadState
            {
                Id = @event.MatchId.Value,
                Home = @event.HomeTeam,
                Away = @event.AwayTeam,
                Series = @event.Series,
                StartTime = @event.StartTime,
                CreatedAtUtc = @event.CreatedAtUtc,
                IsActive = true,
                LastOddsUpdateAtUtc = null,
                StoppedAtUtc = null
            },
            (_, existing) =>
            {
                existing.Home = @event.HomeTeam;
                existing.Away = @event.AwayTeam;
                existing.Series = @event.Series;
                existing.StartTime = @event.StartTime;
                existing.IsActive = true;
                existing.StoppedAtUtc = null;
                return existing;
            });

        return Task.CompletedTask;
    }

    public Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var state = _matches.GetOrAdd(
            @event.Match.Id,
            _ => new MatchReadState
            {
                Id = @event.Match.Id.Value,
                Home = @event.Match.HomeTeam,
                Away = @event.Match.AwayTeam,
                Series = @event.Match.Series,
                StartTime = @event.Match.StartTime,
                CreatedAtUtc = @event.UpdatedAtUtc,
                IsActive = true,
                LastOddsUpdateAtUtc = null,
                StoppedAtUtc = null
            });

        var snapshot = @event.Match.Odds
            .Select(bookmaker => new OddsReadModel(
                bookmaker.Name,
                bookmaker.Outcome.Home,
                bookmaker.Outcome.Away,
                bookmaker.Outcome.Draw,
                bookmaker.UpdatedAt))
            .ToList();

        state.Home = @event.Match.HomeTeam;
        state.Away = @event.Match.AwayTeam;
        state.Series = @event.Match.Series;
        state.StartTime = @event.Match.StartTime;
        state.IsActive = true;
        state.StoppedAtUtc = null;
        state.LastOddsUpdateAtUtc = @event.UpdatedAtUtc;
        state.OddsSnapshots.Enqueue(snapshot);

        return Task.CompletedTask;
    }

    public Task HandleAsync(MatchStoppedEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_matches.TryGetValue(@event.Id, out var state))
        {
            state.IsActive = false;
            state.StoppedAtUtc = @event.StoppedAtUtc;
        }

        return Task.CompletedTask;
    }

    private static MatchReadModel ToReadModel(MatchReadState state)
    {
        var flattenedOdds = state.OddsSnapshots.SelectMany(snapshot => snapshot).ToList();

        return new MatchReadModel(
            state.Id,
            state.Home,
            state.Away,
            state.Series,
            state.StartTime,
            state.IsActive,
            state.CreatedAtUtc,
            state.LastOddsUpdateAtUtc,
            state.StoppedAtUtc,
            flattenedOdds);
    }
}