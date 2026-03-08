namespace TradingEngine.Domain;

public interface ISportEventMessage
{
    public string SportEventId { get; init; }
    Task ApplyAsync(SportEventActor actor);
}