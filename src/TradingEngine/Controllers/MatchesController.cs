using Microsoft.AspNetCore.Mvc;
using TradingEngine.Domain.Matches;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController(IMatchRepository matchRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var matches = await matchRepository.GetAllAsync();
        return Ok(matches);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var match = await matchRepository.GetById(id);
        return Ok(match);
    }
}