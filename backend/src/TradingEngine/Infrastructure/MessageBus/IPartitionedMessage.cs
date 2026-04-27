namespace TradingEngine.Infrastructure.MessageBus;

public interface IPartitionedMessage : IMessage
{
    string PartitionKey { get; }
}