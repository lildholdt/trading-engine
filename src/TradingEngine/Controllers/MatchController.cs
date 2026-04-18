using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class MatchController(IMatchActorSystem actorSystem) : ControllerBase
{
    [HttpGet("matches")]
    public IActionResult GetMatches()
    {
        var events = actorSystem.GetStates();
        return Ok(events);
    }
    
    [HttpGet("matches/{id}")]
    public IActionResult GetEvent(Guid id)
    {
        var events = actorSystem.GetState(id);
        return Ok(events);
    }
    
    [HttpDelete("matches/{id}")]
    public IActionResult StopMatch(Guid id)
    {
        actorSystem.StopAsync(id);
        return Ok();
    }
    
    [HttpPost("matches/reset")]
    public IActionResult Reset()
    {
        actorSystem.Reset();
        return Ok();
    }
}