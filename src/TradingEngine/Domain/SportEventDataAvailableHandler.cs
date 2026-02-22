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
            StartDate = @event.DateTime,
            League = @event.League,
            Sport = @event.Sport,
            Team1 = @event.Team1,
            Team2 = @event.Team2
        };
        
        await repository.SaveAsync(sportEvent, cancellationToken);
        logger.LogInformation("Match found {EventTeam1} vs {EventTeam2}", @event.Team1, @event.Team2);
        await hubPublisher.PublishAsync(sportEvent, cancellationToken);
    }
}