namespace TradingEngine.Domain.Matches.StopMatch;

public record MatchRemovedHubEvent(Guid Id, DateTime StoppedAtUtc);
