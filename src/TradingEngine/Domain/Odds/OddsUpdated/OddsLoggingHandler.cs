using TradingEngine.Clients.Polymarket;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.Odds.OddsUpdated;

public class OddsLoggingHandler(
    IOddsWriter oddsWriter,
    IPolymarketClient polymarketClient,
    IEventRegistry eventRegistry,
    ILogger<OddsLoggingHandler> logger) : IEventHandler<OddsUpdatedEvent>
{
    public async Task HandleAsync(OddsUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var item = eventRegistry.Get(@event.Id);
        if (item == null)
        {
            logger.LogError("Couldn't find event {id}", @event.Id);
            return;
        }
        
        var polymarketEvent = await polymarketClient.GetEvent(item.Id);
        
        var records = @event.Odds.Select(record => new OddsRecord
        {
            Id = @event.Id,
            Home = item!.HomeTeam,
            Away = item!.AwayTeam,
            Bookmaker = record.Name,
            StartTime = item.StartTime,
            SnapshotTime = DateTime.Now,
            HoursBefore = item.StartTime - DateTime.Now,
            TrueOddsHome = record.TrueOdds(OutcomeType.Home),
            TrueOddsAway = record.TrueOdds(OutcomeType.Away),
            TrueOddsDraw = record.TrueOdds(OutcomeType.Draw),
            PolymarketOutcome = 0
        }).ToList();

        await oddsWriter.WriteRecords(records, cancellationToken);
    }

    public record OddsRecord
    {
        public required string Id { get; init; }
        public required DateTime StartTime { get; init; }
        public required string Home { get; init; }
        public required string Away { get; init; }
        public required string Bookmaker { get; init; }
        public required DateTime SnapshotTime { get; init; }
        public required TimeSpan HoursBefore { get; init; }
        public required decimal TrueOddsHome { get; init; }
        public required decimal TrueOddsAway { get; init; }
        public required decimal TrueOddsDraw { get; init; }
        public required decimal PolymarketOutcome { get; init; }
    }
}