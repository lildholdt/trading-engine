namespace TradingEngine.Infrastructure.EventBus;

public interface IEventBus
{   
    public ValueTask PublishAsync(IEvent command, CancellationToken cancellationToken = default);
}