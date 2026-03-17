using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.RegistryItemCorrelated;

public class RegistryItemCorrelatedEvent : IEvent
{
    public required EventRegistryItem Item { get; init; }
}