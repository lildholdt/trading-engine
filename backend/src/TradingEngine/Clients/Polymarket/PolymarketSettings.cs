namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// Configuration settings for Polymarket client integration.
/// </summary>
public class PolymarketSettings
{
    /// <summary>
    /// Base URL for Polymarket API requests.
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Indicates whether to use the local stub implementation.
    /// </summary>
    public bool Mock { get; init; }
}