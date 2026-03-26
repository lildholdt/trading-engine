using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.Events.RegistryItemCorrelated;

public class RegistryItemCorrelatedEvent : IEvent
{
    public required EventRegistryItem Item { get; init; }
}