namespace TradingEngine.Domain;

public class SportEventActorState
{
    public required string Id { get; init; }
    public required string HomeTeam { get; init; }
    public required string AwayTeam { get; init; }
    public required DateTime StartTime { get; init; }
    public IReadOnlyCollection<Bookmaker> Odds { get; init; } = [];
}
