namespace TradingEngine;

public class ApplicationSettings
{
    public int LongRunningRequestThresholdInMs { get; init; } = 3000;
    public string? HubUrl { get; init; }
    public string? OddsApiUrl { get; init; }
}