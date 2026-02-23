using Microsoft.AspNetCore.Mvc;
using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;
using TradingEngine.Domain.MarketUpdate;
using TradingEngine.Domain.SportEventCatalogueEntryAdded;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class SportsController(
    IEventBus eventBus,
    ISportEventActorSystem actorSystem,
    IPolyMarketApiClient client,
    IHubPublisher<SportEventCatalogueEntry> hub) : ControllerBase
{
    [HttpGet("sports")]
    public async Task<IActionResult> GetSports()
    {
        var events = await client.GetSports();
        return Ok(events);
    }
    
    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] int seriesId)
    {
        var events = await client.GetEvents(seriesId);
        return Ok(events);
    }
    
    [HttpGet("events-stream")]
    public async Task<IActionResult> GetEventStream([FromQuery] int seriesId)
    {
        await client.StreamEvents(seriesId, @event =>
        {
            Console.WriteLine(@event.Title);
        });
        return Ok();
    }
    
    [HttpGet("publish")]
    public async Task<IActionResult> Publish()
    {
        var sport = new SportEventCatalogueEntry("1")
        {
            StartTime = DateTime.Now,
            Sport = "soccer",
            League = "test",
            Team1 = "team1",
            Team2 = "team2"
        };
        
        await hub.PublishAsync(sport);
        return Ok();
    }
    
    [HttpPost("sport-event")]
    public async Task<IActionResult> CreateSportEvent()
    {
        var eventData = new SportEventCatalogueEntry("TestId")
        {
            StartTime = DateTime.Now,
            League = "TestLeague",
            Sport = "TestSport",
            Team1 = "Team1",
            Team2 = "Team2",
        };
       
        await eventBus.PublishAsync(new SportEventCatalogueEntryAdded {SportEvent =  eventData});
        return Ok();
    }
    
    [HttpPost("market-update")]
    public async Task<IActionResult> CreateMarketUpdate()
    {
        var marketUpdateMessage = new MarketUpdateMessage("TestId") { HomeOdds = 2 };
        await actorSystem.SendAsync(marketUpdateMessage);
        return Ok();
    }
}