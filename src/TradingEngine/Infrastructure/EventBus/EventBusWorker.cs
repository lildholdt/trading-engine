namespace TradingEngine.Infrastructure.EventBus;

public class EventBusWorker(
    EventBus bus,
    IServiceProvider serviceProvider,
    ILogger<CommandBus.CommandBus> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(EventBus)} started");
        
        await foreach (var @event in bus.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var handlerType = typeof(IEnumerable<>)
                    .MakeGenericType(
                        typeof(IEventHandler<>)
                            .MakeGenericType(@event.GetType()));

                var handlers = (IEnumerable<object>)
                    scope.ServiceProvider
                        .GetRequiredService(handlerType);

                foreach (var handler in handlers)
                {
                    await ((dynamic)handler)
                        .HandleAsync((dynamic)@event, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, 
                    "Error handling event {EventType}", 
                    @event.GetType().Name);
            }
        }
    }
}