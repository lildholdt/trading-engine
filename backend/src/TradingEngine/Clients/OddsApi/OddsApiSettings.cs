namespace TradingEngine.Clients.OddsApi;

public class OddsApiSettings
{
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
    public bool Mock { get; init; }
}