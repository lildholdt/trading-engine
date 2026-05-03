namespace TradingEngine.Domain.Matches;

public record LiveOddsReadModel(string Name, decimal Home, decimal Away, decimal Draw, DateTime UpdatedAt);

public record LiveMatchReadModel(
    Guid Id,
    string Home,
    string Away,
    string Series,
    DateTime StartTime,
    bool IsPaused,
    IReadOnlyCollection<LiveOddsReadModel> Odds);