using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// This static class contains extension methods for Polymarket-related models such as Event, Market, 
/// and collections of Market objects. These extensions add convenience methods for common operations.
/// </summary>
public static class PolymarketModelExtensions
{
    extension(Event @event)
    {
        /// <summary>
        /// Checks if the Event contains any markets of type "moneyline" or where the market type is null.
        /// This is useful for determining if such markets exist within the Event.
        /// </summary>
        public bool HasMoneyLineMarketTypes => 
            @event.Markets.Any(x => x.SportsMarketType is "moneyline" or null);
        
        /// <summary>
        /// Retrieves all markets within the Event that are of type "moneyline" or where the market type is null.
        /// Returns these markets as a read-only collection.
        /// </summary>
        public IReadOnlyCollection<Market> MoneyLineMarkets => 
            @event.Markets.Where(x => x.SportsMarketType is "moneyline" or null).ToList();
    }
    
    extension(IReadOnlyCollection<Market> markets)
    {
        /// <summary>
        /// Retrieves the first Market object from the collection that matches the provided 
        /// groupItemTitle. If no such Market exists, it returns null.
        /// </summary>
        /// <param name="groupItemTitle">The title of the group item to search for.</param>
        /// <returns>The matching Market object or null if no match is found.</returns>
        public Market? Get(string groupItemTitle) => 
            markets.FirstOrDefault(x => x.GroupItemTitle == groupItemTitle);
    }
    
    extension(Market market)
    {
        /// <summary>
        /// Creates an Outcome object for the Market using the first elements from the 
        /// Outcomes, OutcomePrices, and ClobTokenIds collections of the Market. If any of these 
        /// collections are empty, the respective property in the Outcome will be null.
        /// </summary>
        public Outcome Outcome => new()
        {
            Name = market.Outcomes.FirstOrDefault(),              // Retrieves the first outcome name.
            Price = market.OutcomePrices.FirstOrDefault(),        // Retrieves the first outcome price.
            ClobTokenId = market.ClobTokenIds.FirstOrDefault()    // Retrieves the first Clob token ID.
        };
    }
}