using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Clients.Polymarket.Models;
using TradingEngine.Domain;

namespace TradingEngine.Services.Registry;

public class RegistryItem
{
    public required SportEventId Id { get; init; }
    public required Event PolymarketEvent { get; init; }
    public required string HomeTeam { get; init; }
    public required string AwayTeam { get; init; }
    public required DateTime StartTime { get; init;}
    public Odds? OddsApiEvent { get; set; }
}