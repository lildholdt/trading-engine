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
        // TODO: Add simulation call to Polymarket and log volume
        // TODO: Add Polymarket prices
        
        var @event = registry.Get(command.Id);

        var averageHome = Math.Round(command.Odds.Sum(x => x.TrueOdds(OutcomeType.Home)) / command.Odds.Count, 2);
        var averageAway = Math.Round(command.Odds.Sum(x => x.TrueOdds(OutcomeType.Away)) / command.Odds.Count, 2);
        var averageDraw = Math.Round(command.Odds.Sum(x => x.TrueOdds(OutcomeType.Draw)) / command.Odds.Count, 2);
        
        // TODO: Add logic to call order placement API in Polymarket
        
        logger.LogInformation(
            "Order has been placed. HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, " +
            "AverageHome={AverageHome}, AverageAway={AverageAway}, AverageDraw={AverageDraw}",
            @event?.HomeTeam,
            @event?.AwayTeam,
            averageHome,
            averageAway,
            averageDraw
        );
        
        return Task.CompletedTask;
    }
}