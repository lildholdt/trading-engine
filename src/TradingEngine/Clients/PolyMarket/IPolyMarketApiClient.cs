// Importing the namespace containing the models used in the PolyMarket API client.
using TradingEngine.Clients.PolyMarket.Models;

namespace TradingEngine.Clients.PolyMarket
{
    // Interface definition for the PolyMarket API client.
    public interface IPolyMarketApiClient
    {
        // Method to fetch a collection of sports.
        // Returns a task that resolves to an enumerable of SportEntry objects.
        Task<IEnumerable<SportEntry>> GetSports();

        // Method to fetch events associated with a specific series.
        // Accepts the seriesId as an input parameter and returns a task 
        // that resolves to an enumerable of Event objects.
        Task<IEnumerable<Event>> GetEvents(int seriesId);

        // Method to stream events associated with a specific series in real-time.
        // Accepts the seriesId as an input parameter and an Action delegate
        // to handle each streamed Event object.
        Task StreamEvents(int seriesId, Action<Event> action);
    }
}