using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.OddsUpdated;

public class OddsUpdatedEventHandler(ICommandBus commandBus) :  IEventHandler<OddsUpdatedEvent>
{
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    { 
        await commandBus.SendAsync(new PlaceOrderCommand
        {
            Id = @event.Id,
            Odds = @event.Bookmakers
        }, cancellationToken);
    }
}