using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Services;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/system")]
[Authorize]
public class SystemController(ISystemState systemState) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { isRunning = systemState.IsRunning });
    }

    [HttpPost("start")]
    public IActionResult Start()
    {
        systemState.Start();
        return Ok(new { isRunning = systemState.IsRunning });
    }

    [HttpPost("stop")]
    public IActionResult Stop()
    {
        systemState.Stop();
        return Ok(new { isRunning = systemState.IsRunning });
    }
}
