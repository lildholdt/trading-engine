using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.Polymarket;

/// <summary>
/// Enumeration-like value object representing supported Polymarket series.
/// </summary>
public class PolymarketSeries : Enumeration<PolymarketSeries>
{
    // =======================
    // EUROPE - Major Leagues
    // =======================
    public static readonly PolymarketSeries EnglishPremierLeague = new("10188", "English Premier League");
    public static readonly PolymarketSeries LaLiga = new("10193", "La Liga");
    public static readonly PolymarketSeries Bundesliga = new("10194", "Bundesliga");
    public static readonly PolymarketSeries Bundesliga2 = new("10670", "Bundesliga 2");
    public static readonly PolymarketSeries SerieA = new("10203", "Serie A");
    public static readonly PolymarketSeries SerieB = new("10676", "Serie B");
    public static readonly PolymarketSeries Ligue1 = new("10195", "Ligue 1");
    public static readonly PolymarketSeries Ligue2 = new("10675", "Ligue 2");
    public static readonly PolymarketSeries Eredivisie = new("10286", "Eredivisie");

    // =======================
    // EUROPE - Domestic Cups
    // =======================
    public static readonly PolymarketSeries FaCup = new("10307", "FA Cup");
    public static readonly PolymarketSeries Efl = new("10230", "English Football League");
    public static readonly PolymarketSeries CoppaItalia = new("10287", "Coppa Italia");
    public static readonly PolymarketSeries CopaDelRey = new("10316", "Coppa Del Rey");
    public static readonly PolymarketSeries CoupeDeFrance = new("10315", "Coupe De France");

    // =======================
    // EUROPE - Other Domestic Leagues
    // =======================
    public static readonly PolymarketSeries DenmarkSuperLiga = new("73", "Danish Super Liga");
    public static readonly PolymarketSeries RussianPremierLeague = new("10306", "Russian Premier League");
    public static readonly PolymarketSeries TurkishLeague = new("10292", "Turkish League");

    // =======================
    // NORTH & CENTRAL AMERICA
    // =======================
    public static readonly PolymarketSeries Mls = new("10189", "MLS");
    public static readonly PolymarketSeries LigaMx = new("10290", "Liga MX");
    public static readonly PolymarketSeries LeaguesCup = new("10288", "Leagues Cup");

    // =======================
    // SOUTH AMERICA - Domestic Leagues
    // =======================
    public static readonly PolymarketSeries ArgentinaLeague = new("10285", "Argentina League");

    // =======================
    // SOUTH AMERICA - Club Competitions
    // =======================
    public static readonly PolymarketSeries CopaLibertadores = new("10289", "Copa Libertadores");
    public static readonly PolymarketSeries CopaSudamericana = new("10291", "Copa Sudamericana");
    public static readonly PolymarketSeries Conmebol = new("10246", "CONMEBOL");

    // =======================
    // INTERNATIONAL CLUB COMPETITIONS (UEFA)
    // =======================
    public static readonly PolymarketSeries ChampionsLeague = new("10204", "UEFA Champions League");
    public static readonly PolymarketSeries EuropaLeague = new("10209", "UEFA Europa League");

    // =======================
    // INTERNATIONAL NATIONAL TEAMS
    // =======================
    public static readonly PolymarketSeries Fifa = new("10238", "FIFA Competitions");
    public static readonly PolymarketSeries WorldCupQualifiers = new("10243", "World Cup Qualifiers");
    public static readonly PolymarketSeries AfricaCupOfNations = new("10786", "Africa Cup of Nations");

    // =======================
    // CONFEDERATIONS
    // =======================
    public static readonly PolymarketSeries Caf = new("10240", "CAF Competitions");
    public static readonly PolymarketSeries Concacaf = new("10244", "CONCACAF");
    public static readonly PolymarketSeries Uefa = new("10241", "AFC Competitions");
    public static readonly PolymarketSeries Ofc = new("10294", "OFC Competitions");

    /// <summary>
    /// Initializes an empty instance for framework and reflection scenarios.
    /// </summary>
    public PolymarketSeries() { }

    /// <summary>
    /// Initializes a series with identifier and display name.
    /// </summary>
    /// <param name="seriesId">Polymarket series identifier.</param>
    /// <param name="displayName">Human-readable series name.</param>
    private PolymarketSeries(string seriesId, string displayName) : base(seriesId, displayName) { }
}

