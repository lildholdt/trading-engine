using TradingEngine.Infrastructure.Dispatcher;

namespace TradingEngine.Domain.Matches.Reset;

/// <summary>
/// Command used to stop and remove all active matches.
/// </summary>
public class ResetMatchesCommand : ICommand<Unit>;
