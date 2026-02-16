using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.Dispatcher;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class SportsController(
    IDispatcher dispatcher, 
    IEventBus eventBus,
    IPolyMarketApiClient client,
    IHubPublisher<SportEvent> hub) : ControllerBase
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
    
    [HttpGet("dispatch")]
    public IActionResult Dispatch()
    {
        var order = new NewSportEventDataAvailable("Test", DateTime.Now);
        dispatcher.Enqueue(order);
        return Ok();
    }
    
    [HttpGet("publish")]
    public async Task<IActionResult> Publish()
    {
        var sport = new SportEvent(1)
        {
            DateTime = DateTime.Now,
            Sport = "soccer",
            League = "test",
            Team1 = "team1",
            Team2 = "team2",
            Market = "market",
            MarketDetail = 0,
            Outcome1 = 0,
            Outcome2 = 0,
            OutcomeX = 0,
            Odds1 = 0,
            Odds2 = 0,
            OddsX = 0
        };
        
        await hub.PublishAsync(sport);
        
        //await hub.Clients.All.SendAsync("SportEvent", "Team 1 vs Team 2", DateTime.Now);
        return Ok();
    }
}