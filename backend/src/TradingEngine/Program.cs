using TradingEngine;

await WebApplication
    .CreateBuilder(args)
    .Bootstrap()
    .Build()
    .Configure()
    .RunAsync();