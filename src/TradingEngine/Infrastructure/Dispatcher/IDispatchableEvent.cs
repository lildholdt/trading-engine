namespace TradingEngine.Infrastructure.Dispatcher;

// Interface definition for a dispatchable event.
public interface IDispatchableEvent
{
    // Property representing the partition key for the event.
    // It is used to determine how the event should be distributed or categorized.
    // The 'init' accessor ensures that the property can only be set during object initialization.
    string PartitionKey { get; }
}