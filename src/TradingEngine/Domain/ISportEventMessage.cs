namespace TradingEngine.Domain;

public interface ISportEventMessage
{
    public SportEventId SportEventId { get; init; }
    Task ApplyAsync(SportEventActor actor);
}