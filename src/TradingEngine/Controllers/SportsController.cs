using Microsoft.AspNetCore.Mvc;
using TradingEngine.Clients.PolyMarket;
using TradingEngine.Clients.PolyMarket.Models;
using TradingEngine.Domain;
using TradingEngine.Domain.MarketUpdate;
using TradingEngine.Infrastructure.Hub;
using TradingEngine.Services.PolyMarket;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class SportsController(
    IPolymarketEventCatalogue polymarketEventCatalogue,
    ISportEventActorSystem actorSystem,
    IPolymarketApiClient client,
    IHubPublisher<Event> hub) : ControllerBase
{
    [HttpGet("sports")]
    public async Task<IActionResult> GetSports()
    {
        var events = await client.GetSports();
        return Ok(events);
    }
    
    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] string seriesId)
    {
        var events = await client.GetEvents(seriesId);
        return Ok(events);
    }
    
    [HttpGet("events-stream")]
    public async Task<IActionResult> GetEventStream([FromQuery] string seriesId)
    {
        await client.StreamEvents(seriesId, @event =>
        {
            Console.WriteLine(@event.Title);
        });
        return Ok();
    }
    
    // [HttpGet("publish")]
    // public async Task<IActionResult> Publish()
    // {
    //     var sport = new PolyMarketEventCatalogueEntry("1")
    //     {
    //         StartTime = DateTime.Now,
    //         Sport = "soccer",
    //         League = "test",
    //         Team1 = "team1",
    //         Team2 = "team2"
    //     };
    //     
    //     await hub.PublishAsync(sport);
    //     return Ok();
    // }
    
    // [HttpPost("sport-event")]
    // public async Task<IActionResult> CreateSportCatalogueEntry()
    // {
    //     var catalogueEntry = new Event("TestId")
    //     {
    //         StartTime = DateTime.Now,
    //         League = "TestLeague",
    //         Sport = "TestSport",
    //         Team1 = "Team1",
    //         Team2 = "Team2",
    //     };
    //                     
    //     // Save to the catalogue
    //     await polyMarketEventCatalogue.Add(catalogueEntry);
    //     return Ok();
    // }
    
    [HttpPost("market-update")]
    public async Task<IActionResult> CreateMarketUpdate()
    {
        var marketUpdateMessage = new MarketUpdateMessage("TestId") { HomeOdds = 2 };
        await actorSystem.SendAsync(marketUpdateMessage);
        return Ok();
    }
}