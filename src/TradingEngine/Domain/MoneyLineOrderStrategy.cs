using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Domain;

public class MoneyLineOrderStrategy : IOrderStrategy
{
    private double _buffer = 0.03;
    private readonly IEnumerable<string> _bookmakers = new List<string>
    {
        //"betfair_ex_uk", "betfair_sb_uk", "matchbook", "smarkets",
        "pinnacle",
    };
    
    public decimal CalculatePrice(IEnumerable<Bookmaker> bookmakers)
    {
        var filteredBookmakers = bookmakers.Where(b => _bookmakers.Contains(b.Name));

        var trueOddsList = new List<TrueOdds>();
        foreach (var bookmaker in filteredBookmakers)
        {
            // Calculate margin
            var margin = bookmaker.Outcomes.Sum(outcome => 1 / outcome.Value) - 1;

            // Calculate True Odds
            trueOddsList.AddRange(bookmaker.Outcomes.Select(outcome => 
                new TrueOdds
                {
                    Type = outcome.Key, 
                    Bookmaker = bookmaker.Name,
                    Price = 3 * outcome.Value / (3 - margin * outcome.Value)
                }));
        }

        var prices = trueOddsList.Where(x => x.Type == OutcomeType.Home).ToList();
        var price = prices.Sum(x => x.Price) / prices.Count;
        return Math.Round(price * (decimal)(1 + _buffer), 2); 
    }

    private record TrueOdds
    {
        public required OutcomeType Type { get; init; }
        public required string Bookmaker { get; init; }
        public required decimal Price { get; init; }
    }
}