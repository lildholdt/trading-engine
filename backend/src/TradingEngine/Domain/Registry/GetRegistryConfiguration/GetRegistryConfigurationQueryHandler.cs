using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Registry.GetRegistryConfiguration;

public class GetRegistryConfigurationQueryHandler(IRegistry registry)
    : IQueryHandler<GetRegistryConfigurationQuery, IReadOnlyCollection<RegistryConfigurationItemReadModel>>
{
    public Task<IReadOnlyCollection<RegistryConfigurationItemReadModel>> HandleAsync(GetRegistryConfigurationQuery query,
        CancellationToken cancellationToken = default)
    {
        var config = registry.GetConfiguration();
        var models = config
            .Select(x => new RegistryConfigurationItemReadModel(x.Id, x.Name, x.Active))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<RegistryConfigurationItemReadModel>>(models);
    }
}
