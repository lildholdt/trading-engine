using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Registry;

/// <summary>
/// Event published when a registry item is successfully correlated across data sources.
/// </summary>
public class RegistryItemCorrelatedEvent : IEvent
{
    /// <summary>
    /// Gets the correlated registry item.
    /// </summary>
    public required RegistryItem Item { get; init; }
}