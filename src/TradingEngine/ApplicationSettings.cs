namespace TradingEngine;

public class ApplicationSettings
{
    
    public int LongRunningRequestThresholdInMs { get; init; } = 3000;
    public TeamMatching TeamMatching { get; init; } = new();
    public OddsApi OddsApi { get; init; } = new();
}

public class TeamMatching
{
    public double Threshold { get; init; } = 0.9;
    public double TokenWeight { get; init; } = 0.4;
    public double FuzzyWeight { get; init; } = 0.4;
    public double NoSpaceWeight { get; init; } = 0.2;
}

public class OddsApi
{
    public string BaseUrl { get; init; }
    public string ApiKey { get; init; }
}