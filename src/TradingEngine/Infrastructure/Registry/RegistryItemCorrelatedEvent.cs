using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Infrastructure.Registry;

/// <summary>
/// Event published when a registry item is successfully correlated across data sources.
/// </summary>
public class RegistryItemCorrelatedEvent : IEvent
{
    /// <summary>
    /// Gets the correlated registry item.
    /// </summary>
    public required MatchRegistryItem Item { get; init; }
}