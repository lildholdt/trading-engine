namespace TradingEngine.Infrastructure.Hub;

// Interface definition for a hub publisher that publishes events.
// It is generic and works with a specific type of event, constrained to be a reference type (class).
public interface IHubPublisher<in TEvent> where TEvent : class
{
    // Asynchronous method to publish the specified event.
    // Parameters:
    // - @event: The event to be published.
    Task PublishAsync(TEvent @event);
}