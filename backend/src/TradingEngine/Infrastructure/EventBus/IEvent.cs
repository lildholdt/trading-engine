using TradingEngine.Infrastructure.MessageBus;

namespace TradingEngine.Infrastructure.EventBus;

/// <summary>
/// Represents a marker interface for events in the event bus system.
/// </summary>
public interface IEvent : IMessage;