using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.Polymarket;

namespace TradingEngine.Infrastructure.Registry;

public class EventRegistryConfiguration
{
    private int _currentConfigurationId;
    private readonly List<EventRegistryConfigurationItem> _config = [];

    public EventRegistryConfiguration()
    {
        AddConfiguration("English Premier League", PolymarketSeries.EnglishPremierLeague, OddsApiSportsType.EnglishPremierLeague);
        AddConfiguration("World Cup Qualifiers", PolymarketSeries.WorldCupQualifiers, OddsApiSportsType.WorldCupQualifiers);
    }

    private void AddConfiguration(string name, string polymarketSeriesId, string oddsApiSportsType)
    {
        _config.Add(new EventRegistryConfigurationItem
        {
            Id = ++_currentConfigurationId,
            Name = name,
            PolymarketSeriesId = polymarketSeriesId,
            OddsApiSportsType = oddsApiSportsType
        });
    }

    public IReadOnlyList<EventRegistryConfigurationItem> GetAll()
    {
        return _config.AsReadOnly();
    }

    public void Update(int id, bool state)
    {
        var index = _config.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Configuration item with id '{id}' was not found.");
        }

        var current = _config[index];
        _config[index] = new EventRegistryConfigurationItem
        {
            Id = current.Id,
            Name = current.Name,
            PolymarketSeriesId = current.PolymarketSeriesId,
            OddsApiSportsType = current.OddsApiSportsType,
            Active = state
        };
    }
}