using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.Polymarket;

namespace TradingEngine.Domain.Registry;

public class RegistryConfiguration
{
    private int _currentConfigurationId;
    private readonly List<RegistryMapping> _config = [];

    public RegistryConfiguration()
    {
        // =======================
        // EUROPE - Major Leagues
        // =======================
        AddConfiguration("English Premier League", PolymarketSeries.EnglishPremierLeague, OddsApiSportsType.EnglishPremierLeague);
        AddConfiguration("La Liga", PolymarketSeries.LaLiga, OddsApiSportsType.LaLiga);
        AddConfiguration("Bundesliga", PolymarketSeries.Bundesliga, OddsApiSportsType.Bundesliga);
        AddConfiguration("Bundesliga 2", PolymarketSeries.Bundesliga2, OddsApiSportsType.Bundesliga2);
        AddConfiguration("Serie A", PolymarketSeries.SerieA, OddsApiSportsType.SerieA);
        AddConfiguration("Serie B", PolymarketSeries.SerieB, OddsApiSportsType.SerieB);
        AddConfiguration("Ligue 1", PolymarketSeries.Ligue1, OddsApiSportsType.Ligue1);
        AddConfiguration("Ligue 2", PolymarketSeries.Ligue2, OddsApiSportsType.Ligue2);
        AddConfiguration("Eredivisie", PolymarketSeries.Eredivisie, OddsApiSportsType.Eredivisie);

        // =======================
        // EUROPE - Domestic Cups
        // =======================
        AddConfiguration("FA Cup", PolymarketSeries.FaCup, OddsApiSportsType.FaCup);
        AddConfiguration("EFL", PolymarketSeries.Efl, OddsApiSportsType.Efl);
        AddConfiguration("Coppa Italia", PolymarketSeries.CoppaItalia, OddsApiSportsType.CoppaItalia);
        AddConfiguration("Copa del Rey", PolymarketSeries.CopaDelRey, OddsApiSportsType.CopaDelRey);
        AddConfiguration("Coupe de France", PolymarketSeries.CoupeDeFrance, OddsApiSportsType.CoupeDeFrance);

        // =======================
        // EUROPE - Other Domestic Leagues
        // =======================
        AddConfiguration("Danish Superliga", PolymarketSeries.DenmarkSuperLiga, OddsApiSportsType.DenmarkSuperLiga);
        AddConfiguration("Turkish League", PolymarketSeries.TurkishLeague, OddsApiSportsType.TurkeySuperLeague);
        AddConfiguration("Russian Premier League", PolymarketSeries.RussianPremierLeague, OddsApiSportsType.RussianPremierLeague);

        // =======================
        // AMERICAS - Domestic Leagues
        // =======================
        AddConfiguration("MLS", PolymarketSeries.Mls, OddsApiSportsType.Mls);
        AddConfiguration("Liga MX", PolymarketSeries.LigaMx, OddsApiSportsType.LigaMx);
        AddConfiguration("Argentina League", PolymarketSeries.ArgentinaLeague, OddsApiSportsType.ArgentinaLeague);

        // =======================
        // AMERICAS - Cups / Regional
        // =======================
        AddConfiguration("Leagues Cup", PolymarketSeries.Mls, OddsApiSportsType.Mls);

        // =======================
        // SOUTH AMERICA - Domestic
        // =======================
        AddConfiguration("Brazil Serie A", PolymarketSeries.ArgentinaLeague, OddsApiSportsType.ArgentinaLeague);

        // =======================
        // SOUTH AMERICA - International Club
        // =======================
        AddConfiguration("Copa Libertadores", PolymarketSeries.CopaLibertadores, OddsApiSportsType.CopaLibertadores);
        AddConfiguration("Copa Sudamericana", PolymarketSeries.CopaSudamericana, OddsApiSportsType.CopaSudamericana);

        // =======================
        // INTERNATIONAL CLUB COMPETITIONS
        // =======================
        AddConfiguration("UEFA Champions League", PolymarketSeries.ChampionsLeague, OddsApiSportsType.ChampionsLeague);
        AddConfiguration("UEFA Europa League", PolymarketSeries.EuropaLeague, OddsApiSportsType.EuropaLeague);

        // =======================
        // INTERNATIONAL NATIONAL TEAMS
        // =======================
        AddConfiguration("FIFA Competitions", PolymarketSeries.Fifa, OddsApiSportsType.Fifa);
        AddConfiguration("World Cup Qualifiers", PolymarketSeries.WorldCupQualifiers, OddsApiSportsType.WorldCupQualifiers);
        AddConfiguration("Africa Cup of Nations", PolymarketSeries.AfricaCupOfNations, OddsApiSportsType.AfricaCupOfNations);

        // =======================
        // CONFEDERATIONS
        // =======================
        AddConfiguration("AFC Competitions", PolymarketSeries.Uefa, OddsApiSportsType.Uefa);
        AddConfiguration("CAF Competitions", PolymarketSeries.Caf, OddsApiSportsType.Caf);
        AddConfiguration("CONCACAF", PolymarketSeries.Concacaf, OddsApiSportsType.Mls);
        AddConfiguration("CONMEBOL", PolymarketSeries.Conmebol, OddsApiSportsType.CopaLibertadores);
        AddConfiguration("OFC Competitions", PolymarketSeries.Ofc, OddsApiSportsType.Ofc);
    }

    private void AddConfiguration(string name, PolymarketSeries polymarketSeriesId, OddsApiSportsType oddsApiSportsType)
    {
        _config.Add(new RegistryMapping
        {
            Id = ++_currentConfigurationId,
            Name = name,
            PolymarketSeriesId = polymarketSeriesId,
            OddsApiSportsType = oddsApiSportsType
        });
    }

    public IReadOnlyList<RegistryMapping> GetAll()
    {
        return _config.AsReadOnly();
    }

    public void Update(int id, bool state)
    {
        var index = _config.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Configuration item with id '{id}' was not found.");
        }

        var current = _config[index];
        _config[index] = new RegistryMapping
        {
            Id = current.Id,
            Name = current.Name,
            PolymarketSeriesId = current.PolymarketSeriesId,
            OddsApiSportsType = current.OddsApiSportsType,
            Active = state
        };
    }
}