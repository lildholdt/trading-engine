using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;
using TradingEngine.Domain.Messages;
using TradingEngine.Infrastructure.Hub;
using TradingEngine.Services.Registry;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class ManagementController(
    ISportEventActorSystem actorSystem,
    IEventRegistry eventRegistry,
    IHubPublisher<Event> hub) : ControllerBase
{
    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        var events = actorSystem.GetStates();
        return Ok(events);
    }
    
    [HttpDelete("events")]
    public IActionResult StopEvent(string id)
    {
        actorSystem.StopAsync(id);
        return Ok();
    }
    
    [HttpPost("events/reset")]
    public IActionResult Reset()
    {
        actorSystem.Reset();
        return Ok();
    }
    
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
                    }.ToImmutableDictionary()
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