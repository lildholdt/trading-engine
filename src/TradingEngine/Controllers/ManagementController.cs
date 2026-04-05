using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class ManagementController(
    ISportEventActorSystem actorSystem,
    IEventRegistry eventRegistry) : ControllerBase
{
    [HttpGet("registry")]
    public IActionResult GetRegistryItems()
    {
        var items = eventRegistry.GetAll();
        return Ok(items);
    }
    
    [HttpGet("registry/configuration")]
    public IActionResult GetRegistryConfiguration()
    {
        var config = eventRegistry.GetConfiguration();
        return Ok(config);
    }
    
    [HttpPost("registry/configuration/{id}")]
    public IActionResult UpdateRegistryConfiguration(int id, bool state)
    {
        eventRegistry.UpdateConfiguration(id, state);
        return Ok();
    }
    
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