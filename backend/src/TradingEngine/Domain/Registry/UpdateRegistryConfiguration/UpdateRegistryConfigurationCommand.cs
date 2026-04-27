using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.UpdateRegistryConfiguration;

public class UpdateRegistryConfigurationCommand : ICommand<Unit>
{
    public required int Id { get; init; }
    public required bool State { get; init; }
}
