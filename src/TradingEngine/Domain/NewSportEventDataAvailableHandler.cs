using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain;

public class NewSportEventDataAvailableHandler(
    ILogger<NewSportEventDataAvailableHandler> logger,
    IRepository<SportEvent, int> repository) : IDispatchableEventHandler<NewSportEventDataAvailable>
{
    public Task HandleAsync(NewSportEventDataAvailable @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Match found {@event.Title} : {@event.DateTime}");
        return Task.CompletedTask;
    }
}