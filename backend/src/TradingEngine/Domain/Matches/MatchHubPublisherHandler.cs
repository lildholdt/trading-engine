using TradingEngine.Domain.Matches.CreateMatch;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Domain.Matches;

public sealed class MatchHubPublisherHandler(
    IHubPublisher<MatchUpsertedHubEvent> upsertPublisher,
    IHubPublisher<MatchRemovedHubEvent> removedPublisher)
    : IEventHandler<MatchCreatedEvent>,
      IEventHandler<OddsUpdatedEvent>,
      IEventHandler<MatchStoppedEvent>
{
    public Task HandleAsync(MatchCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var live = new Match
        {
            Id = @event.MatchId,
            HomeTeam = @event.HomeTeam,
            AwayTeam = @event.AwayTeam,
            Series = @event.Series,
            StartTime = @event.StartTime,
            IsPaused = false,
            Odds = []
        };

        return upsertPublisher.PublishAsync(new MatchUpsertedHubEvent(live, @event.CreatedAtUtc), cancellationToken);
    }

    public Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var live = @event.Match;

        return upsertPublisher.PublishAsync(new MatchUpsertedHubEvent(live, @event.UpdatedAtUtc), cancellationToken);
    }

    public Task HandleAsync(MatchStoppedEvent @event, CancellationToken cancellationToken = default)
        => removedPublisher.PublishAsync(new MatchRemovedHubEvent(@event.Id.Value, @event.StoppedAtUtc), cancellationToken);
}
