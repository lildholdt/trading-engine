namespace TradingEngine.Domain.Matches.UpdateOdds;

public record MatchUpsertedHubEvent(LiveMatchReadModel Match, DateTime ChangedAtUtc);
