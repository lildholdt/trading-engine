namespace TradingEngine.Infrastructure.Dispatcher;

public interface IDispatchableEvent
{
    string PartitionKey { get; init; }
}