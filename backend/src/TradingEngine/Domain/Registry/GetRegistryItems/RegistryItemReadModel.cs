namespace TradingEngine.Domain.Registry.GetRegistryItems;

public record RegistryItemReadModel(
    Guid Id,
    string PolymarketHome,
    string PolymarketAway,
    string? OddsApiHome,
    string? OddsApiAway,
    double? CorrelationScore);