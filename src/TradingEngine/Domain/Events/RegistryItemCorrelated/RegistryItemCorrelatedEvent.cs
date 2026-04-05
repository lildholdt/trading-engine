using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Domain.Events.RegistryItemCorrelated;

/// <summary>
/// Event published when a registry item is successfully correlated across data sources.
/// </summary>
public class RegistryItemCorrelatedEvent : IEvent
{
    /// <summary>
    /// Gets the correlated registry item.
    /// </summary>
    public required EventRegistryItem Item { get; init; }
}