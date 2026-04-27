using TradingEngine.Domain.Matches.UpdateOdds;

namespace TradingEngine.Domain.Matches;

public interface IOddsRepository
{
    public void Append(OddsUpdatedEvent odds);
}