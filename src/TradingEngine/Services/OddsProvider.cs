using TradingEngine.Clients.OddsApi;
using TradingEngine.Domain;
using TradingEngine.Services.Registry;
using Bookmaker = TradingEngine.Domain.Bookmaker;

namespace TradingEngine.Services;

public class OddsProvider(IOddsApiClient client, IEventRegistry registry) : IOddsProvider
{
    public async Task<IEnumerable<Bookmaker>> GetOdds(SportEventId id)
    {
        var item = registry.Get(id);
        if (item == null) return [];
        
        var odds = await client.GetOdds(item.OddsApiEvent?.Id!);

        var bookmakers = new List<Bookmaker>();
        foreach (var b in odds?.Bookmakers!)
        {
            var market = b.Markets.FirstOrDefault(x => x.Key == "h2h");
            if (market == null) continue;
            
            var bookmaker = new Bookmaker
            {
                Name = b.Key,
                LastUpdate = b.LastUpdate,
                Outcomes = new Dictionary<OutcomeType, decimal>
                {
                    { OutcomeType.Home, market.Outcomes.FirstOrDefault(x => x.Name == item.OddsApiEvent?.HomeTeam)!.Price},
                    { OutcomeType.Away, market.Outcomes.FirstOrDefault(x => x.Name == item.OddsApiEvent?.AwayTeam)!.Price},
                    { OutcomeType.Draw, market.Outcomes.FirstOrDefault(x => x.Name == "Draw")!.Price}
                }
            };
            bookmakers.Add(bookmaker);
        }
        return bookmakers;
    }
}