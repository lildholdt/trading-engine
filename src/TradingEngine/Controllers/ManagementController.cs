using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain.Matches;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/matches/active")]
public class ManagementController(IMatchActorSystem actorSystem) : ControllerBase
{
    [HttpGet]
    public IActionResult GetMatches()
    {
        var events = actorSystem.GetStates();
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetEvent(Guid id)
    {
        var events = actorSystem.GetState(id);
        return Ok(events);
    }
    
    [HttpDelete("{id}")]
    public IActionResult StopMatch(Guid id)
    {
        actorSystem.StopAsync(id);
        return Ok();
    }
    
    [HttpPost("reset")]
    public IActionResult Reset()
    {
        actorSystem.Reset();
        return Ok();
    }
}