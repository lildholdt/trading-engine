namespace TradingEngine.Clients.Polymarket;

public static class ServiceCollectionExtensions
{
	private const string SettingsSection = "Polymarket";
	private const string ResourcePath = "Clients/Polymarket/Resources/polymarket-event.json";
	
	extension(IServiceCollection services)
	{
		public void AddPolymarketClient(IConfiguration configuration)
		{
			services.Configure<PolymarketSettings>(configuration.GetSection(SettingsSection));
			var settings = configuration.GetSection(SettingsSection).Get<PolymarketSettings>();
			
			if (settings?.Mock == true)
			{
				services.AddSingleton<IPolymarketClient>(_ => new PolymarketClientStub(ResourcePath));
				return;
			}

			services.AddHttpClient<IPolymarketClient, PolymarketClient>()
				.AddNamedHttpMessageHandler<LoggingHandler>();
		}
	}
}