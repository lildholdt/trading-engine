using TradingEngine.Infrastructure;

namespace TradingEngine.Clients.OddsApi;

/// <summary>
/// Enumeration-like value object representing supported Odds API sport keys.
/// </summary>
public class OddsApiSportsType : Enumeration<OddsApiSportsType>
{
    /// <summary>
    /// Odds API Sports types.
    /// </summary>
    
    // =======================
    // EUROPE - Major Leagues
    // =======================
    public static readonly OddsApiSportsType EnglishPremierLeague = new("soccer_epl", "English Premier League");
    public static readonly OddsApiSportsType LaLiga = new("soccer_spain_la_liga", "Spanish Soccer");
    public static readonly OddsApiSportsType Bundesliga = new("soccer_germany_bundesliga", "German Soccer");
    public static readonly OddsApiSportsType Bundesliga2 = new("soccer_germany_bundesliga2", "German Soccer");
    public static readonly OddsApiSportsType SerieA = new("soccer_italy_serie_a", "Italian Soccer");
    public static readonly OddsApiSportsType SerieB = new("soccer_italy_serie_b", "Italian Soccer");
    public static readonly OddsApiSportsType Ligue1 = new("soccer_france_ligue_one", "French Soccer");
    public static readonly OddsApiSportsType Ligue2 = new("soccer_france_ligue_two", "French Soccer");
    public static readonly OddsApiSportsType Eredivisie = new("soccer_netherlands_eredivisie", "Dutch Soccer");

    // =======================
    // EUROPE - Domestic Cups
    // =======================
    public static readonly OddsApiSportsType FaCup = new("soccer_fa_cup", "Football Association Challenge Cup");
    public static readonly OddsApiSportsType Efl = new("soccer_efl_champ", "EFL Championship");
    public static readonly OddsApiSportsType CoppaItalia = new("soccer_italy_coppa_italia", "Italian Soccer");
    public static readonly OddsApiSportsType CopaDelRey = new("soccer_spain_copa_del_rey", "Spanish Soccer");
    public static readonly OddsApiSportsType CoupeDeFrance = new("soccer_france_coupe_de_france", "French Soccer");

    // =======================
    // EUROPE - Other Domestic Leagues
    // =======================
    public static readonly OddsApiSportsType DenmarkSuperLiga = new("soccer_denmark_superliga", "Danish Soccer");
    public static readonly OddsApiSportsType RussianPremierLeague = new("soccer_russia_premier_league", "Russian Soccer");
    public static readonly OddsApiSportsType TurkeySuperLeague = new("soccer_turkey_super_league", "Turkish Soccer");

    // =======================
    // NORTH & CENTRAL AMERICA
    // =======================
    public static readonly OddsApiSportsType Mls = new("soccer_usa_mls", "Major League Soccer");
    public static readonly OddsApiSportsType LigaMx = new("soccer_mexico_ligamx", "Mexican Soccer");
    public static readonly OddsApiSportsType LeaguesCup = new("soccer_mexico_ligamx", "Mexican Soccer");

    // =======================
    // SOUTH AMERICA - Domestic Leagues
    // =======================
    public static readonly OddsApiSportsType ArgentinaLeague = new("soccer_argentina_primera_division", "Argentine Primera División");
    public static readonly OddsApiSportsType BrazilSerieA = new("soccer_brazil_campeonato", "Brasileirão Série A");
    public static readonly OddsApiSportsType BrazilSerieB = new("soccer_brazil_serie_b", "Campeonato Brasileiro Série B");
    public static readonly OddsApiSportsType ChilePrimera = new("soccer_chile_campeonato", "Campeonato Chileno");

    // =======================
    // SOUTH AMERICA - Club Competitions
    // =======================
    public static readonly OddsApiSportsType CopaLibertadores = new("soccer_conmebol_copa_libertadores", "CONMEBOL Copa Libertadores");
    public static readonly OddsApiSportsType CopaSudamericana = new("soccer_conmebol_copa_sudamericana", "CONMEBOL Copa Sudamericana");
    public static readonly OddsApiSportsType Conmebol = new("soccer_conmebol_copa_libertadores", "CONMEBOL");

    // =======================
    // INTERNATIONAL CLUB COMPETITIONS (UEFA)
    // =======================
    public static readonly OddsApiSportsType ChampionsLeague = new("soccer_uefa_champs_league", "UEFA Champions League");
    public static readonly OddsApiSportsType EuropaLeague = new("soccer_uefa_europa_league", "UEFA Europa League");
    public static readonly OddsApiSportsType EuropaConferenceLeague = new("soccer_uefa_europa_conference_league", "UEFA Europa Conference League");

    // =======================
    // INTERNATIONAL NATIONAL TEAMS
    // =======================
    public static readonly OddsApiSportsType Fifa = new("soccer_fifa_world_cup", "FIFA World Cup 2026");
    public static readonly OddsApiSportsType WorldCupQualifiers = new("soccer_fifa_world_cup", "World Cup Qualifiers");
    public static readonly OddsApiSportsType AfricaCupOfNations = new("soccer_africa_cup_of_nations", "Africa Cup of Nations");

    // =======================
    // CONFEDERATIONS
    // =======================
    public static readonly OddsApiSportsType Caf = new("soccer_africa_cup_of_nations", "CAF Competitions");
    public static readonly OddsApiSportsType Concacaf = new("soccer_north_america", "CONCACAF");
    public static readonly OddsApiSportsType Uefa = new("soccer_uefa_champs_league", "UEFA");
    public static readonly OddsApiSportsType Ofc = new("soccer_oceania", "OFC Competitions");
        
    /// <summary>
    /// Initializes an empty instance for framework and reflection scenarios.
    /// </summary>
    public OddsApiSportsType() { }

    /// <summary>
    /// Initializes a sport type using the Odds API identifier and display name.
    /// </summary>
    /// <param name="identifier">The Odds API sport key.</param>
    /// <param name="displayName">Human-readable sport name.</param>
    private OddsApiSportsType(string identifier, string displayName) : base(identifier, displayName) { }
}

