namespace TradingEngine.Infrastructure.Registry;

public class EventRegistryConfigurationItem
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string PolymarketSeriesId { get; init; }
    public required string OddsApiSportsType { get; init; }
    public bool Active { get; init; } = false;
}