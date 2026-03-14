using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Domain;

public class MoneyLineOrderStrategy : IOrderStrategy
{
    private double _buffer = 0.03;
    private readonly IEnumerable<string> _bookmakers = new List<string>
    {
        //"betfair_ex_uk", "betfair_sb_uk", "matchbook", "smarkets", "betfair_ex_eu",
        "pinnacle",
    };
    
    public decimal CalculatePrice(Odds odds)
    {
        var filteredBookmakers = odds.Bookmakers.Where(b => _bookmakers.Contains(b.Key));

        var trueOddsList = new List<TrueOdds>();
        foreach (var bookmaker in filteredBookmakers)
        {
            var market = bookmaker.Markets.FirstOrDefault(x => x.Key == "h2h");
            if (market == null) continue;

            // Calculate margin
            var margin = market.Outcomes.Sum(outcome => 1 / outcome.Price) - 1;

            // Calculate True Odds
            trueOddsList.AddRange(market.Outcomes.Select(outcome => 
                new TrueOdds
                {
                    Name = outcome.Name, 
                    Bookmaker = bookmaker.Key,
                    Price = 3 * outcome.Price / (3 - margin * outcome.Price)
                }));
        }

        var prices = trueOddsList.Where(x => x.Name == odds.HomeTeam).ToList();
        var price = prices.Sum(x => x.Price) / prices.Count;
        return Math.Round(price * (decimal)(1 + _buffer), 2); 
    }

    private record TrueOdds
    {
        public required string Name { get; init; }
        public required string Bookmaker { get; init; }
        public required decimal Price { get; init; }
    }
}