namespace TradingEngine.Infrastructure.Hub;

public interface IHubPublisher<in TEvent> where TEvent : class
{
    Task PublishAsync(TEvent @event);
}