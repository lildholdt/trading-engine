namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// A hosted service responsible for managing the lifecycle of the Dispatcher.
/// This service starts the Dispatcher when the application starts and stops it gracefully when the application shuts down.
/// </summary>
public class DispatcherService(Dispatcher dispatcher, ILogger<DispatcherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // All workers already running inside the dispatcher
        logger.LogInformation("Dispatcher started");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}