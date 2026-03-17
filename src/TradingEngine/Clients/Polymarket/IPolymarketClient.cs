// Importing the namespace containing the models used in the PolyMarket API client.

using TradingEngine.Clients.Polymarket.Models;

namespace TradingEngine.Clients.Polymarket
{
    // Interface definition for the PolyMarket API client.
    public interface IPolymarketClient
    {
        // Method to fetch a collection of sports.
        // Returns a task that resolves to an enumerable of SportEntry objects.
        Task<IEnumerable<SportEntry>> GetSports();

        // Method to fetch events associated with a specific series.
        // Accepts the seriesId as an input parameter and returns a task 
        // that resolves to an enumerable of Event objects.
        Task<IEnumerable<Event>> GetEvents(string seriesId);

        // Method to stream events associated with a specific series in real-time.
        // Accepts the seriesId as an input parameter and an Action delegate
        // to handle each streamed Event object.
        Task StreamEvents(string seriesId, Action<Event> action);
    }
}