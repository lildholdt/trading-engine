using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain.Registry.GetRegistryConfiguration;
using TradingEngine.Domain.Registry.GetRegistryItems;
using TradingEngine.Domain.Registry.UpdateRegistryConfiguration;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/registry")]
[Authorize]
public class RegistryController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRegistryItems(
        [FromQuery] string? search,
        [FromQuery] string? sortBy = "home",
        [FromQuery] string? sortDirection = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var models = await dispatcher.Query(new GetRegistryItemsQuery
        {
            Search = search,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize
        });
        return Ok(models);
    }
    
    [HttpGet("configuration")]
    public async Task<IActionResult> GetRegistryConfiguration()
    {
        var models = await dispatcher.Query(new GetRegistryConfigurationQuery());
        return Ok(models);
    }
    
    [HttpPost("configuration/{id}")]
    public async Task<IActionResult> UpdateRegistryConfiguration(int id, bool state)
    {
        await dispatcher.Send(new UpdateRegistryConfigurationCommand
        {
            Id = id,
            State = state
        });
        return Ok();
    }
}