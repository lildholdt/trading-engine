namespace TradingEngine.Domain;

public interface ISportEventCommand
{
    public string SportEventId { get; init; }
    Task ApplyAsync(SportEventActor actor);
}