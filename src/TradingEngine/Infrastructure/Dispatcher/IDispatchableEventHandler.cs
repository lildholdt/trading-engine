namespace TradingEngine.Infrastructure.Dispatcher;

public interface IDispatchableEventHandler<in TEvent> where TEvent : IDispatchableEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}