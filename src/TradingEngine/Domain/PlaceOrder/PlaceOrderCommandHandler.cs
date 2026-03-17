using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.PlaceOrder;

public class PlaceOrderCommandHandler(
    IPolymarketClient polymarketClient, 
    IEventRegistry registry,
    ILogger<PlaceOrderCommandHandler> logger) 
    : ICommandHandler<PlaceOrderCommand> 
{
    public Task HandleAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        var @event = registry.Get(command.Id);
        logger.LogInformation("Order has been placed. Price={CommandPrice}, HomeTeam={EventHomeTeam}, AwayTeam={EventAwayTeam}", 
            command.Price, @event?.HomeTeam, @event?.AwayTeam);
        return Task.CompletedTask;
    }
}