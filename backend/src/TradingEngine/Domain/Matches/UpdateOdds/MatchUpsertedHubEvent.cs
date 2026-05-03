namespace TradingEngine.Domain.Matches.UpdateOdds;

public record MatchUpsertedHubEvent(Match Match, DateTime ChangedAtUtc);
