using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.GetRegistryConfiguration;

public class GetRegistryConfigurationQuery : IQuery<IReadOnlyCollection<RegistryConfigurationItemReadModel>>;
