namespace TradingEngine.Infrastructure.CommandBus;

public class CommandBusWorker(
    CommandBus bus,
    IServiceProvider serviceProvider,
    ILogger<CommandBus> logger)
    : BackgroundService
{
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