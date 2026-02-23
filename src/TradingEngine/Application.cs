using Microsoft.OpenApi;
using Serilog;
using TradingEngine.Clients;
using TradingEngine.Clients.PolyMarket;
using TradingEngine.Domain;
using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Domain.SportEventCatalogueEntryAdded;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Infrastructure.Hub;
using TradingEngine.Services;
using TradingEngine.Utils;

namespace TradingEngine;

public static class Application
{
    /// <summary>
    /// Bootstrapping of the service.
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <returns>A <see cref="WebApplicationBuilder"/>The web application builder with the default service bootstrapping applied</returns>
    public static WebApplicationBuilder Bootstrap(this WebApplicationBuilder builder)
    {
        // Bind the ApplicationSettings section to the ApplicationSettings class
        builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
        
        // Register dispatcher
        // builder.Services.AddSingleton<IDispatcher>(sp => sp.GetRequiredService<Dispatcher>());
        // builder.Services.AddSingleton<Dispatcher>();
        // builder.Services.AddHostedService<DispatcherService>();
        
        // Register event bus
        builder.Services.AddHostedService<EventBusWorker>();
        builder.Services.AddSingleton<EventBus>();
        builder.Services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<EventBus>());
        builder.Services.AddSingleton<IEventHandler<SportEventCatalogueEntryAdded>, SportEventCatalogueEntryAddedHandler>();
        
        // Register command bus
        builder.Services.AddHostedService<CommandBusWorker>();
        builder.Services.AddSingleton<CommandBus>();
        builder.Services.AddSingleton<ICommandBus>(sp => sp.GetRequiredService<CommandBus>());
        builder.Services.AddSingleton<ICommandHandler<PlaceOrderCommand>,  PlaceOrderCommandHandler>();
        
        // Register actor system
        builder.Services.AddSingleton<ISportEventActorSystem,  SportEventActorSystem>();
        
        // Register repositories for entities
        builder.Services.AddSingleton(typeof(IRepository<,>), typeof(InMemoryRepository<,>));
        builder.Services.AddSingleton<ISportEventCatalogue, SportEventCatalogue>();
        
        // Register SignalR hub publisher
        builder.Services.AddSingleton(typeof(IHubPublisher<>), typeof(HubPublisher<>));
        
        // Register services
        // builder.Services.AddHostedService<PolyMarketSyncService>();
        
        
        // Register utils
        builder.Services.AddSingleton<ITeamMatcher, DeterministicTeamMatcher>();
        
        // Register clients
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<LoggingHandler>();
        builder.Services.AddHttpClient<IPolyMarketApiClient, PolyMarketApiClient>().AddNamedHttpMessageHandler<LoggingHandler>();
        
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()   
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error) // Disable Microsoft logs
            .MinimumLevel.Override("Microsoft.Hosting", Serilog.Events.LogEventLevel.Information) // Enable Microsoft.Hosting information logs
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error) // Disable System logs
            .CreateLogger();
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sport betting",
                Version = "v1",
                Description = "Automatic sports betting"
            });
        });
        
        // Add CORS services and configure the allowed origins
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => string.IsNullOrEmpty(origin) || origin == "null" || true) // Allow null origins
                    .AllowCredentials(); // Allow credentials (if required)
            });
        });
        
        // Add Serilog to the logging pipeline
        builder.Host.UseSerilog();
        
        return builder;
    }
    
    /// <summary>
    /// Configuration of the service
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application with the default configuration applied</returns>
    public static WebApplication Configure(this WebApplication app)
    {
        // Enable Swagger in development environments.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
        });
        
        // Use CORS middleware
        app.UseCors("AllowAll");
        
        
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<GenericEventHub>("/hubs/trading");
        return app;
    }
}