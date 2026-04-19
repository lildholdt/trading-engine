using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Orders;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController(
    IMatchReadRepository matchRepository,
    IOrdersRepository ordersRepository,
    IMatchActorSystem actorSystem) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var matches = await matchRepository.GetAllAsync();
        return Ok(matches);
    }
    
    [HttpGet("{id}/odds")]
    public async Task<IActionResult> GetOddsById(Guid id)
    {
        var match = await matchRepository.GetOddsAsync(id);
        return Ok(match);
    }
    
    [HttpGet("{id}/orders")]
    public IActionResult GetByMatchId(Guid id)
    {
        var orders = ordersRepository.GetOrders(id);
        return Ok(orders);
    }
    
    [HttpGet("{id}/orders-csv")]
    public IActionResult GetByMatchIdCsv(Guid id)
    {
        var orders = ordersRepository.GetOrders(id);

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