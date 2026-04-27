using TradingEngine.Infrastructure.MessageBus;

namespace TradingEngine.Infrastructure.CommandBus;

/// <summary>
/// Represents a command message that can be sent through the command bus.
/// </summary>
public interface ICommand : IMessage;