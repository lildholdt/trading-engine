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
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    // /// <summary>
    // /// Called when the service is starting. 
    // /// Initiates the Dispatcher and logs relevant information.
    // /// </summary>
    // /// <param name="cancellationToken">A token to signal cancellation of the start operation.</param>
    // /// <returns>A completed task indicating the service has started successfully.</returns>
    // public Task StartAsync(CancellationToken cancellationToken)
    // {
    //     
    //     // All workers already running inside the dispatcher
    //     logger.LogInformation("Dispatcher service started");
    //     return Task.CompletedTask;
    // }
    //
    // /// <summary>
    // /// Called when the service is stopping. 
    // /// Cancels the ongoing operations in the Dispatcher and logs relevant information.
    // /// </summary>
    // /// <param name="cancellationToken">A token to signal cancellation of the stop operation.</param>
    // /// <returns>A completed task indicating the service has stopped successfully.</returns>
    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     logger.LogInformation("Stopping dispatcher service");
    //    // _cts.Cancel();
    //     return Task.CompletedTask;
    // }
}