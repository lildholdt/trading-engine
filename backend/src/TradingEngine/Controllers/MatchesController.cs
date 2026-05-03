using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Matches.GetMatchOdds;
using TradingEngine.Domain.Matches.GetMatches;
using TradingEngine.Domain.Matches.PauseMatch;
using TradingEngine.Domain.Matches.Reset;
using TradingEngine.Domain.Matches.ResumeMatch;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Orders.GetOrders;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController(IDispatcher dispatcher, IMatchActorSystem actorSystem) : ControllerBase
{
    [HttpGet("live")]
    public async Task<IActionResult> GetLive(
        [FromQuery] string? search,
        [FromQuery] DateTime? startTimeFromUtc,
        [FromQuery] DateTime? startTimeToUtc,
        [FromQuery] string? sortByStartTime = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var matches = await actorSystem.GetAllLiveAsync();
        IEnumerable<LiveMatchReadModel> filtered = matches;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            filtered = filtered.Where(m =>
                m.Home.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                m.Away.Contains(normalized, StringComparison.OrdinalIgnoreCase));
        }

        if (startTimeFromUtc.HasValue)
        {
            filtered = filtered.Where(m => m.StartTime >= startTimeFromUtc.Value);
        }

        if (startTimeToUtc.HasValue)
        {
            filtered = filtered.Where(m => m.StartTime <= startTimeToUtc.Value);
        }

        var sortDesc = string.Equals(sortByStartTime, "desc", StringComparison.OrdinalIgnoreCase);
        filtered = sortDesc
            ? filtered.OrderByDescending(m => m.StartTime)
            : filtered.OrderBy(m => m.StartTime);

        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 50 : pageSize;

        var result = filtered
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(match => new
            {
                id = match.Id,
                home = match.Home,
                away = match.Away,
                series = match.Series,
                startTime = match.StartTime,
                isPaused = match.IsPaused,
                odds = match.Odds.Select(bookmaker => new
                {
                    name = bookmaker.Name,
                    home = bookmaker.Home,
                    away = bookmaker.Away,
                    draw = bookmaker.Draw,
                    updatedAt = bookmaker.UpdatedAt
                })
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("live/{id:guid}")]
    public async Task<IActionResult> GetLiveById(Guid id)
    {
        var match = await actorSystem.GetLiveByIdAsync(id);
        if (match == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = match.Id,
            home = match.Home,
            away = match.Away,
            series = match.Series,
            startTime = match.StartTime,
            isPaused = match.IsPaused,
            odds = match.Odds.Select(bookmaker => new
            {
                name = bookmaker.Name,
                home = bookmaker.Home,
                away = bookmaker.Away,
                draw = bookmaker.Draw,
                updatedAt = bookmaker.UpdatedAt
            })
        });
    }

    [HttpPost("live/{id}/pause")]
    public async Task<IActionResult> PauseMatch(Guid id)
    {
        await dispatcher.Send(new PauseMatchCommand { MatchId = id });
        return Ok();
    }

    [HttpPost("live/{id}/resume")]
    public async Task<IActionResult> ResumeMatch(Guid id)
    {
        await dispatcher.Send(new ResumeMatchCommand { MatchId = id });
        return Ok();
    }
    
    [HttpDelete("live/{id}")]
    public async Task<IActionResult> StopMatch(Guid id)
    {
        await dispatcher.Send(new StopMatchCommand { MatchId = id });
        return Ok();
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string? search,
        [FromQuery] DateTime? startTimeFromUtc,
        [FromQuery] DateTime? startTimeToUtc,
        [FromQuery] string? sortByStartTime = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var matches = await dispatcher.Query(new GetMatchesQuery
        {
            Search = search,
            StartTimeFromUtc = startTimeFromUtc,
            StartTimeToUtc = startTimeToUtc,
            SortByStartTime = sortByStartTime,
            Page = page,
            PageSize = pageSize
        });

        return Ok(matches);
    }

    [HttpGet("history/{id:guid}")]
    public async Task<IActionResult> GetHistoryById(Guid id)
    {
        var matches = await dispatcher.Query(new GetMatchesQuery
        {
            Page = 1,
            PageSize = int.MaxValue
        });

        var match = matches.FirstOrDefault(m => m.Id == id);
        return match == null ? NotFound() : Ok(match);
    }

    [HttpGet("{id}/odds")]
    public async Task<IActionResult> GetOddsById(Guid id)
    {
        var match = await dispatcher.Query(new GetMatchOddsQuery { MatchId = id });
        return Ok(match);
    }
    
    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetOrders(Guid id)
    {
        var orders = await dispatcher.Query(new GetOrdersQuery { MatchId = id });
        return Ok(orders);
    }
    
    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await dispatcher.Send(new ResetMatchesCommand());
        return Ok();
    }
}