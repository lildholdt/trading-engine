using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Dispatcher;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Domain;

public class SportEventDataAvailableHandler(
    ILogger<SportEventDataAvailableHandler> logger,
    IRepository<SportEvent, string> repository,
    IHubPublisher<SportEvent> hubPublisher) : IDispatchableEventHandler<SportEventDataAvailable>
{
    public async Task HandleAsync(SportEventDataAvailable @event, CancellationToken cancellationToken = default)
    {
        var sportEvent = new SportEvent(@event.Id)
        {
            Id = @event.Id,
            DateTime = @event.DateTime,
            League = @event.League,
            Sport = @event.Sport,
            Team1 = @event.Team1,
            Team2 = @event.Team2,
            Market = null,
            MarketDetail = 0,
            Outcome1 = 0,
            Outcome2 = 0,
            OutcomeX = 0,
            Odds1 = 0,
            Odds2 = 0,
            OddsX = 0
        };
        
        await repository.SaveAsync(sportEvent, cancellationToken);
        logger.LogInformation("Match found {EventTeam1} vs {EventTeam2}", @event.Team1, @event.Team2);
        await hubPublisher.PublishAsync(sportEvent, cancellationToken);
    }
}