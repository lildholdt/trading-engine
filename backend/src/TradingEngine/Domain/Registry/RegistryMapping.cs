using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.Polymarket;

namespace TradingEngine.Domain.Registry;

public record RegistryMapping
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required PolymarketSeries PolymarketSeriesId { get; init; }
    public required OddsApiSportsType OddsApiSportsType { get; init; }
    public bool Active { get; init; } = false;
}