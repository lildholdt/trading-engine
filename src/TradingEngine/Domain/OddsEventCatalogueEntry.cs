using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public class OddsEventCatalogueEntry(string id) : Entity<string>(id)
{
    public string Id { get; set; }
    public DateTime CommenceTime { get; set; }
    public string League { get; set; }
    public string Sport { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
}
