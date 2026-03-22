using TradingEngine.Infrastructure.CommandBus;

namespace TradingEngine.Domain.PlaceOrder;

public class PlaceOrderCommand : ICommand 
{
    public required SportEventId Id { get; init; }
    public required IReadOnlyCollection<Bookmaker> Odds { get; init; }
}