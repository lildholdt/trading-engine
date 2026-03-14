using System.Text.Json;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Domain;
using Xunit;

namespace TradingEngine.UnitTests.Domain;

public class MoneyLineOrderStrategyTest
{
    private readonly Odds? _odds;
    private readonly MoneyLineOrderStrategy _strategy;
    
    public MoneyLineOrderStrategyTest()
    {
        const string filePath = "Domain/odds.json";

        // Read the JSON file
        var jsonData = File.ReadAllText(filePath);

        // Deserialize the JSON into an Odds object
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Handles camelCase to PascalCase conversion
            PropertyNameCaseInsensitive = true, // Makes property names case-insensitive
            AllowTrailingCommas = true // Allows trailing commas (useful for debugging JSON)
        };

        _odds = JsonSerializer.Deserialize<Odds>(jsonData, options);

        // Initialize strategy
        _strategy = new MoneyLineOrderStrategy();
    }
    
    [Fact]
    public void MatchScore_ShouldReturnHighScore_ForSimilarNames()
    {
        //var placeOrder = _strategy.CalculatePrice(_odds!);
    }
}