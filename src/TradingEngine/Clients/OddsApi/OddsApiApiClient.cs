namespace TradingEngine.Clients.OddsApi;

public class OddsApiClient(HttpClient httpClient, ILogger<OddsApiClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<OddsApiClient> _logger = logger;
    
    
}