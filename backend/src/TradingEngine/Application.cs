using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TradingEngine.Clients;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.Polymarket;
using TradingEngine.Domain.Matches;
using TradingEngine.Domain.Matches.CreateMatch;
using TradingEngine.Domain.Matches.GetMatchOdds;
using TradingEngine.Domain.Matches.GetMatches;
using TradingEngine.Domain.Matches.PauseMatch;
using TradingEngine.Domain.Matches.Reset;
using TradingEngine.Domain.Matches.ResumeMatch;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Domain.Orders;
using TradingEngine.Domain.Orders.GetOrders;
using TradingEngine.Domain.Registry;
using TradingEngine.Domain.Registry.GetRegistryConfiguration;
using TradingEngine.Domain.Registry.GetRegistryItems;
using TradingEngine.Domain.Registry.UpdateRegistryConfiguration;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.Dispatcher;
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
        
        // Register utils
        builder.Services.AddSingleton<ITeamMatcher, DeterministicTeamMatcher>();
        builder.Services.AddSingleton<IOddsWriter>(_ => new OrderWriter("odds.csv"));

        // Register asynchronous event bus
        builder.Services.AddHostedService<EventBusWorker>();
        builder.Services.AddSingleton<EventBus>();
        builder.Services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<EventBus>());
        builder.Services.AddSingleton<IEventHandler<RegistryItemCorrelatedEvent>,  MatchCreationHandler>();
        builder.Services.AddSingleton<IEventHandler<OddsUpdatedEvent>, OrderPlacementHandler>();
        builder.Services.AddSingleton<InMemoryMatchReadRepository>();
        builder.Services.AddSingleton<MatchLiveHubPublisherHandler>();
        builder.Services.AddSingleton<IEventHandler<MatchCreatedEvent>>(sp => sp.GetRequiredService<InMemoryMatchReadRepository>());
        builder.Services.AddSingleton<IEventHandler<MatchCreatedEvent>>(sp => sp.GetRequiredService<MatchLiveHubPublisherHandler>());
        builder.Services.AddSingleton<IEventHandler<OddsUpdatedEvent>>(sp => sp.GetRequiredService<InMemoryMatchReadRepository>());
        builder.Services.AddSingleton<IEventHandler<OddsUpdatedEvent>>(sp => sp.GetRequiredService<MatchLiveHubPublisherHandler>());
        builder.Services.AddSingleton<IEventHandler<MatchStoppedEvent>>(sp => sp.GetRequiredService<InMemoryMatchReadRepository>());
        builder.Services.AddSingleton<IEventHandler<MatchStoppedEvent>>(sp => sp.GetRequiredService<MatchLiveHubPublisherHandler>());
        
        // Register asynchronous command bus
        builder.Services.AddHostedService<CommandBusWorker>();
        builder.Services.AddSingleton<CommandBus>();
        builder.Services.AddSingleton<ICommandBus>(sp => sp.GetRequiredService<CommandBus>());
        
        // Register synchronous dispatcher
        builder.Services.AddScoped<IDispatcher, Dispatcher>();
        builder.Services.AddScoped<ICommandHandler<PauseMatchCommand, Unit>, PauseMatchCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<ResumeMatchCommand, Unit>, ResumeMatchCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<StopMatchCommand, Unit>, StopMatchCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<ResetMatchesCommand, Unit>, ResetMatchesCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateOddsCommand, Unit>, UpdateOddsCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateRegistryConfigurationCommand, Unit>, UpdateRegistryConfigurationCommandHandler>();
        builder.Services.AddScoped<IQueryHandler<GetMatchesQuery, IReadOnlyCollection<MatchReadModel>>, GetMatchesQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetMatchOddsQuery, IReadOnlyCollection<OddsReadModel>>, GetMatchOddsQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderReadModel>>, GetOrdersQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetRegistryItemsQuery, IReadOnlyCollection<RegistryItemReadModel>>, GetRegistryItemsQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetRegistryConfigurationQuery, IReadOnlyCollection<RegistryConfigurationItemReadModel>>, GetRegistryConfigurationQueryHandler>();
        
        // Register actor system
        builder.Services.AddSingleton<IMatchActorSystem,  MatchActorSystem>();
        
        // Register repositories for entities
        builder.Services.AddSingleton(typeof(IRepository<,>), typeof(InMemoryRepository<,>));
        builder.Services.AddSingleton<MatchRepository>();
        builder.Services.AddSingleton<IMatchRepository>(sp => sp.GetRequiredService<MatchRepository>());
        builder.Services.AddSingleton<IMatchReadRepository>(sp => sp.GetRequiredService<InMemoryMatchReadRepository>());
        builder.Services.AddSingleton<IOrdersRepository, OrdersRepository>();
        
        // Register SignalR hub publisher
        builder.Services.AddSingleton(typeof(IHubPublisher<>), typeof(HubPublisher<>));
        
        // Register clients
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<LoggingHandler>();
        builder.Services.AddPolymarketClient(builder.Configuration);
        builder.Services.AddOddsApiClient(builder.Configuration);

        var jwtKey = builder.Configuration["Auth:JwtKey"] ?? "SuperSecretChangeMe1234567890123456";
        var jwtIssuer = builder.Configuration["Auth:Issuer"] ?? "TradingEngine";
        var jwtAudience = builder.Configuration["Auth:Audience"] ?? "TradingEngine.Client";

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        builder.Services.AddAuthorization();
        
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        
        // Register system state
        builder.Services.AddSingleton<SystemState>();
        builder.Services.AddSingleton<ISystemState>(sp => sp.GetRequiredService<SystemState>());

        // Register services
        builder.Services.AddHostedService<OddsApiMatchSyncService>();
        builder.Services.AddHostedService<PolymarketMatchSyncService>();
        builder.Services.AddSingleton<IRegistry, InMemoryRegistry>();
        builder.Services.AddSingleton<IOddsProvider, OddsProvider>();

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

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", 
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
                Description = "Enter JWT token in the format: Bearer {token}"
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<GenericEventHub>("/hubs/trading");
        return app;
    }
}