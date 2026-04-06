using Microsoft.AspNetCore.Mvc;
using TradingEngine.Infrastructure.Registry;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api")]
public class RegistryController(IEventRegistry eventRegistry) : ControllerBase
{
    private record RegistryItemModel(Guid Id, string PolymarketHome, string PolymarketAway, string? OddsApiHome, string?  OddsApiAway, double? CorrelationScore);

    private record RegistryConfigurationItemModel(int Id, string Name, bool Active);
    
    [HttpGet("registry")]
    public IActionResult GetRegistryItems()
    {
        var items = eventRegistry.GetAll();
        var models = items.Select(x => 
            new RegistryItemModel(
                x.Id.Value, 
                x.HomeTeam, 
                x.AwayTeam, 
                x.OddsApiEvent?.HomeTeam, 
                x.OddsApiEvent?.AwayTeam, 
                x.CorrelationScore));
        return Ok(models);
    }
    
    [HttpGet("registry/configuration")]
    public IActionResult GetRegistryConfiguration()
    {
        var config = eventRegistry.GetConfiguration();
        var models = config.Select(x => new RegistryConfigurationItemModel(x.Id, x.Name, x.Active));
        return Ok(models);
    }
    
    [HttpPost("registry/configuration/{id}")]
    public IActionResult UpdateRegistryConfiguration(int id, bool state)
    {
        eventRegistry.UpdateConfiguration(id, state);
        return Ok();
    }
}