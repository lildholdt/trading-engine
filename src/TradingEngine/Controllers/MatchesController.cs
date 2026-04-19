using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using TradingEngine.Domain.Matches.GetMatchOdds;
using TradingEngine.Domain.Matches.GetMatches;
using TradingEngine.Domain.Matches.Reset;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Orders.GetOrders;
using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
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
    
    [HttpGet("{id}/orders-csv")]
    public async Task<IActionResult> GetByMatchIdCsv(Guid id)
    {
        var orders = await dispatcher.Query(new GetOrdersQuery { MatchId = id });

        var csv = new StringBuilder();
        csv.AppendLine("Id,Bookmaker,SnapshotTime,HoursBefore,OddsHome,OddsDraw,OddsAway,TrueOddsHome,TrueOddsDraw,TrueOddsAway,TrueOddsAverageHome,TrueOddsAverageDraw,TrueOddsAverageAway,PolymarketOutcomeHome,PolymarketOutcomeDraw,PolymarketOutcomeAway");

        foreach (var order in orders)
        {
            csv.AppendLine(string.Join(",", [
                CsvEscape(order.Id),
                CsvEscape(order.Bookmaker),
                CsvEscape(order.SnapshotTime.ToString("O", CultureInfo.InvariantCulture)),
                CsvEscape(order.HoursBefore.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.OddsHome.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.OddsDraw.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.OddsAway.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsHome.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsDraw.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsAway.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsAverageHome.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsAverageDraw.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.TrueOddsAverageAway.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.PolymarketOutcomeHome.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.PolymarketOutcomeDraw.ToString(CultureInfo.CurrentCulture)),
                CsvEscape(order.PolymarketOutcomeAway.ToString(CultureInfo.CurrentCulture))
            ]));
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"orders-{id}.csv");
    }

    private static string CsvEscape(string value)
    {
        // Always quote values to reduce Excel auto-conversion surprises.
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> StopMatch(Guid id)
    {
        await dispatcher.Send(new StopMatchCommand { MatchId = id });
        return Ok();
    }
    
    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await dispatcher.Send(new ResetMatchesCommand());
        return Ok();
    }
}