using TradingEngine.Infrastructure.CommandBus;

namespace TradingEngine.Domain.PlaceOrder;

public class PlaceOrderCommand : ICommand 
{
    public required string Id { get; init; }
}