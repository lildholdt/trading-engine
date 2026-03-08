namespace TradingEngine.Infrastructure.CommandBus;

public interface ICommandBus
{   
    public ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default);
}