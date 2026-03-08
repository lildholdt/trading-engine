using TradingEngine.Clients.PolyMarket;
using TradingEngine.Infrastructure.CommandBus;

namespace TradingEngine.Domain.PlaceOrder;

public class PlaceOrderCommandHandler(
    IPolyMarketApiClient polyMarketApiClient, 
    ILogger<PlaceOrderCommandHandler> logger) 
    : ICommandHandler<PlaceOrderCommand> 
{
    public Task HandleAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Order has been placed");
        return Task.CompletedTask;
    }
}