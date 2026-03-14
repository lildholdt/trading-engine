using Microsoft.AspNetCore.Mvc;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;
using TradingEngine.Domain.UpdateOdds;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class SportsController(
    ISportEventActorSystem actorSystem,
    IHubPublisher<Event> hub) : ControllerBase
{
    [HttpPost("odds")]
    public async Task<IActionResult> UpdateOdds(
        [FromBody] UpdateOddsBody odds)
    {
        var marketUpdateMessage = new UpdateOddsMessage
        {
            SportEventId = new SportEventId(odds.SportEventId), 
            Bookmakers = [new Bookmaker
                {
                    Name = odds.Bookmaker,
                    LastUpdate =  DateTime.Now,
                    Outcomes = new Dictionary<OutcomeType, decimal>
                    {
                        { OutcomeType.Home, odds.Home },
                        { OutcomeType.Away, odds.Away },
                        { OutcomeType.Draw, odds.Draw }
                    }
                }
            ]
        };
        await actorSystem.SendAsync(marketUpdateMessage);
        return Ok();
    }
    
    public class UpdateOddsBody
    {
        public Guid SportEventId { get; set; }
        public string Bookmaker { get; set; }
        public decimal Home { get; set; }
        public decimal Away { get; set; }
        public decimal Draw { get; set; }
    }
}