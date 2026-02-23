namespace TradingEngine.Infrastructure.MessageBus;

public interface IMessageBus<in TMessage> where TMessage : IMessage
{
    ValueTask SendAsync(TMessage message, CancellationToken cancellationToken);
}