namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// Service registration helpers for Polymarket clients.
/// </summary>
public static class ServiceCollectionExtensions
{
	private const string SettingsSection = "Polymarket";
	private const string ResourcePath = "Clients/Polymarket/Resources/polymarket-events.json";
	
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Registers Polymarket client services based on configuration.
		/// </summary>
		/// <param name="configuration">Application configuration source.</param>
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