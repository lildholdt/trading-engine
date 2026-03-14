using TradingEngine.Clients.OddsApi.Models;

namespace TradingEngine.Domain;

public interface IOrderStrategy
{
    public decimal CalculatePrice(Odds odds);
}