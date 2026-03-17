namespace TradingEngine.Domain;

public interface IOrderStrategy
{
    public decimal CalculatePrice(IEnumerable<Bookmaker> odds);
}