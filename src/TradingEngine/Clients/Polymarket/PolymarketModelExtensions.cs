using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

public static class PolymarketModelExtensions
{
    extension(Event @event)
    {
        public bool HasMoneyLineMarketTypes => @event.Markets.Any(x => x.SportsMarketType == "moneyline");
        public bool HasSpreadsMarketTypes => @event.Markets.Any(x => x.SportsMarketType == "spreads");
        public IReadOnlyCollection<Market> GetMoneyLineMarkets() => 
            @event.Markets.Where(x => x.SportsMarketType == "moneyline").ToList();
    }
}