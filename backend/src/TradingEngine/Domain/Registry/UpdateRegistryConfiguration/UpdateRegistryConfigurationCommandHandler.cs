using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.UpdateRegistryConfiguration;

public class UpdateRegistryConfigurationCommandHandler(IRegistry registry)
    : ICommandHandler<UpdateRegistryConfigurationCommand, Unit>
{
    public Task<Unit> HandleAsync(UpdateRegistryConfigurationCommand command,
        CancellationToken cancellationToken = default)
    {
        registry.UpdateConfiguration(command.Id, command.State);
        return Task.FromResult(Unit.Value);
    }
}
