using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain;

public sealed record SportEventDataAvailable : IDispatchableEvent
{
    public string PartitionKey => Id;
    public required string Id { get; init; }
    public required DateTime DateTime { get; init; }
    public required string Sport  { get; init; }
    public required string League { get; init; }
    public required string Team1 { get; init; }
    public required string Team2 { get; init; }
}