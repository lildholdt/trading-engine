using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class SportEventsController(ISportEventActorSystem actorSystem) : ControllerBase
{
    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        var events = actorSystem.GetStates();
        return Ok(events);
    }
    
    [HttpGet("events/{id}")]
    public IActionResult GetEvent(Guid id)
    {
        var events = actorSystem.GetState(id);
        return Ok(events);
    }
    
    [HttpDelete("events/{id}")]
    public IActionResult StopEvent(Guid id)
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
}