using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

public static class PolymarketModelHelper
{
    extension(Event @event)
    {
        public string Home => @event.Title.Split(" vs. ")[0];
        public string Away => @event.Title.Split(" vs. ")[1];

        public bool HasMoneyLineMarketTypes => @event.Markets.Any(x => x.SportsMarketType == "moneyline");
        public bool HasSpreadsMarketTypes => @event.Markets.Any(x => x.SportsMarketType == "spreads");
        
        public IReadOnlyCollection<Market> GetMoneyLineMarkets() => 
            @event.Markets.Where(x => x.SportsMarketType == "moneyline").ToList();
        
        // public Outcome HomeOutcome()
        // {
        //     var market = @event.Markets.FirstOrDefault(x => x.GroupItemTitle == @event.Home);
        // }
    }
}