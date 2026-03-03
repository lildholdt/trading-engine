using System;
using System.Collections.Generic;

namespace TradingEngine.Clients.OddsApi.Models;

public class MatchOdds
{
    public string Id { get; set; }
    public string SportKey { get; set; }
    public string SportTitle { get; set; }
    public DateTime CommenceTime { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public List<Bookmaker> Bookmakers { get; set; }
}

public class Bookmaker
{
    public string Key { get; set; }
    public string Title { get; set; }
    public DateTime LastUpdate { get; set; }
    public List<Market> Markets { get; set; }
}

public class Market
{
    public string Key { get; set; }
    public DateTime LastUpdate { get; set; }
    public List<Outcome> Outcomes { get; set; }
}

public class Outcome
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
