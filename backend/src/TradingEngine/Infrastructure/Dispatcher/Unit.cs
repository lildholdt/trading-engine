namespace TradingEngine.Infrastructure.Dispatcher;

/// <summary>
/// Represents an empty value for commands that do not return domain data.
/// </summary>
public struct Unit
{
    /// <summary>
    /// Shared singleton value for <see cref="Unit"/>.
    /// </summary>
    public static Unit Value { get; } = new();
}