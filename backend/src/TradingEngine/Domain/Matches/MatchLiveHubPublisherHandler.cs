using TradingEngine.Domain.Matches.CreateMatch;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Domain.Matches;

public sealed class MatchLiveHubPublisherHandler(
    IHubPublisher<MatchUpsertedHubEvent> upsertPublisher,
    IHubPublisher<MatchRemovedHubEvent> removedPublisher)
    : IEventHandler<MatchCreatedEvent>,
      IEventHandler<OddsUpdatedEvent>,
      IEventHandler<MatchStoppedEvent>
{
    public Task HandleAsync(MatchCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var live = new LiveMatchReadModel(
            @event.MatchId.Value,
            @event.HomeTeam,
            @event.AwayTeam,
            @event.Series,
            @event.StartTime,
            false,
            []);

        return upsertPublisher.PublishAsync(new MatchUpsertedHubEvent(live, @event.CreatedAtUtc), cancellationToken);
    }

    public Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var live = new LiveMatchReadModel(
            @event.Match.Id.Value,
            @event.Match.HomeTeam,
            @event.Match.AwayTeam,
            @event.Match.Series,
            @event.Match.StartTime,
            @event.Match.IsPaused,
            @event.Match.Odds
                .Select(bookmaker => new LiveOddsReadModel(
                    bookmaker.Name,
                    bookmaker.Outcome.Home,
                    bookmaker.Outcome.Away,
                    bookmaker.Outcome.Draw,
                    bookmaker.UpdatedAt))
                .ToList());

        return upsertPublisher.PublishAsync(new MatchUpsertedHubEvent(live, @event.UpdatedAtUtc), cancellationToken);
    }

    public Task HandleAsync(MatchStoppedEvent @event, CancellationToken cancellationToken = default)
        => removedPublisher.PublishAsync(new MatchRemovedHubEvent(@event.Id.Value, @event.StoppedAtUtc), cancellationToken);
}
