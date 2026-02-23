using TradingEngine.Infrastructure;

namespace TradingEngine.Domain;

public interface ISportEventCatalogue : IRepository<SportEventCatalogueEntry, string>;

public class SportEventCatalogue : InMemoryRepository<SportEventCatalogueEntry, string>, ISportEventCatalogue;