using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain.Registry;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/registry")]
public class RegistryController(IRegistry registry) : ControllerBase
{
    private record RegistryItemModel(Guid Id, string PolymarketHome, string PolymarketAway, string? OddsApiHome, string?  OddsApiAway, double? CorrelationScore);

    private record RegistryConfigurationItemModel(int Id, string Name, bool Active);
    
    [HttpGet]
    public IActionResult GetRegistryItems()
    {
        var items = registry.GetAll();
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
    
    [HttpGet("configuration")]
    public IActionResult GetRegistryConfiguration()
    {
        var config = registry.GetConfiguration();
        var models = config.Select(x => new RegistryConfigurationItemModel(x.Id, x.Name, x.Active));
        return Ok(models);
    }
    
    [HttpPost("configuration/{id}")]
    public IActionResult UpdateRegistryConfiguration(int id, bool state)
    {
        registry.UpdateConfiguration(id, state);
        return Ok();
    }
}