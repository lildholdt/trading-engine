using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.Polymarket;

namespace TradingEngine.Infrastructure.Registry;

public record MatchRegistryMapping
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required PolymarketSeries PolymarketSeriesId { get; init; }
    public required OddsApiSportsType OddsApiSportsType { get; init; }
    public bool Active { get; init; } = false;
}