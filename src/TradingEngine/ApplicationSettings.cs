namespace TradingEngine;

public class ApplicationSettings
{
    public int LongRunningRequestThresholdInMs { get; init; } = 3000;
    public double Threshold { get; init; } = 0.9;
    public double TokenWeight { get; init; } = 0.6;
    public double FuzzyWeight { get; init; } = 0.4;
}