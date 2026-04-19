namespace TradingEngine.Infrastructure.CommandBus;

/// <summary>
/// Background worker that consumes commands from <see cref="CommandBus"/>
/// and dispatches them to their registered handlers.
/// </summary>
public class CommandBusWorker(
    CommandBus bus,
    IServiceProvider serviceProvider,
    ILogger<CommandBus> logger)
    : BackgroundService
{
    /// <summary>
    /// Continuously reads commands from the bus and invokes matching handlers.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token used to stop the worker loop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(CommandBus)} started");
        await foreach (var command in bus.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var handlerType = typeof(ICommandHandler<>)
                    .MakeGenericType(command.GetType());

                var handler = scope.ServiceProvider
                    .GetRequiredService(handlerType);

                await ((dynamic)handler)
                    .HandleAsync((dynamic)command, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, 
                    "Error handling command {CommandType}", 
                    command.GetType().Name);
            }
        }
    }
}