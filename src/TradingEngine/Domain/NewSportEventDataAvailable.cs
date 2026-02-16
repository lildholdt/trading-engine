using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain;

public sealed record NewSportEventDataAvailable (string Title, DateTime DateTime) : IDispatchableEvent
{
    public string PartitionKey { get; init; } = "default";
}