using TradingEngine.Clients.OddsApi;
using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Registry;

namespace TradingEngine.Services;

public class OddsProvider(IOddsApiClient client, IRegistry registry) : IOddsProvider
{
    private readonly IEnumerable<string> _bookmakers = [ 
        "betfair_ex_eu", 
        "betfair_sb_eu", 
        "matchbook", 
        "smarkets",
        "pinnacle"
    ];
    
    public async Task<IReadOnlyCollection<Bookmaker>> GetOdds(MatchId id)
    {
        var item = registry.Get(id);
        if (item?.OddsApiEvent == null) return [];
        
        var sportsType = OddsApiSportsType.FromValue(item.OddsApiEvent.SportKey);
        var odds = await client.GetOdds(sportsType, item.OddsApiEvent.Id);

        if (odds == null) 
            return [];
        
        var bookmakers = new List<Bookmaker>();
        foreach (var b in odds.Bookmakers)
        {
            var market = b.Markets.FirstOrDefault(x => x.Key == "h2h");
            if (market == null) continue;
            
            if(!_bookmakers.Contains(b.Key))
                continue;

            var outcome = new Outcome
            {
                Home = market.Outcomes.FirstOrDefault(x => x.Name == item.OddsApiEvent?.HomeTeam)!.Price,
                Away = market.Outcomes.FirstOrDefault(x => x.Name == item.OddsApiEvent?.AwayTeam)!.Price,
                Draw = market.Outcomes.FirstOrDefault(x => x.Name == "Draw")!.Price
            };
            
            var bookmaker = new Bookmaker(b.Key, outcome);
            bookmakers.Add(bookmaker);
        }
        return bookmakers;
    }
}