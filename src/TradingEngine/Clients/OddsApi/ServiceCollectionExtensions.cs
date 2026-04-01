namespace TradingEngine.Clients.OddsApi;

public static class ServiceCollectionExtensions
{
	private const string SettingsSection = "OddsApi";
	private const string ResourcePath = "Clients/OddsApi/Resources/oddsapi-event.json";
	
	extension(IServiceCollection services)
	{
		public void AddOddsApiClient(IConfiguration configuration)
		{
			services.Configure<OddsApiSettings>(configuration.GetSection(SettingsSection));
			var settings = configuration.GetSection(SettingsSection).Get<OddsApiSettings>();
			
			if (settings?.Mock == true)
			{
				services.AddSingleton<IOddsApiClient>(_ => new OddsApiClientStub(ResourcePath));
			}
			else
			{
				services.AddHttpClient<IOddsApiClient, OddsApiClient>()
					.AddNamedHttpMessageHandler<LoggingHandler>();
			}
		}
	}
}