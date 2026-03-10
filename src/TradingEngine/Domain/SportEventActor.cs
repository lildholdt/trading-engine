using System.Threading.Channels;
using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Infrastructure;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.PolyMarket;

namespace TradingEngine.Domain;

public sealed class SportEventActor
{
    private readonly string _sportEvent;
    private readonly Channel<ISportEventMessage> _mailbox;
    private readonly ICommandBus _commandBus;
    private readonly IEventBus _eventBus;

    // state
    public required DateTime StartTime { get; init; }
    public required string Sport  { get; init; }
    public required string League { get; init; }
    public required string Team1 { get; init; }
    public required string Team2 { get; init; }
    public IEnumerable<Market> Markets { get; init; } = new List<Market>();

    public SportEventActor(
        string sportEvent,
        ICommandBus commandBus,
        IEventBus eventBus)
    {
        _sportEvent = sportEvent;
        _commandBus = commandBus;
        _eventBus = eventBus;

        _mailbox = Channel.CreateUnbounded<ISportEventMessage>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        _ = RunAsync();
    }

    public ValueTask SendAsync(ISportEventMessage message)
        => _mailbox.Writer.WriteAsync(message);

    private async Task RunAsync()
    {
        await foreach (var message in _mailbox.Reader.ReadAllAsync())
        {
            await message.ApplyAsync(this);
        }
    }

    // --- state mutation helpers ---

    public async Task ApplyMarketUpdate(decimal homeOdds)
    {
        if (ShouldPlaceBet())
        {
            await _commandBus.SendAsync(new PlaceOrderCommand { Id = _sportEvent });
        }
    }

    private bool ShouldPlaceBet()
        => true;
    
    
    public class Market : ValueObject
    {
        public required int Id { get; init; }
        public required DateTime StartDate { get; init; }
        public required IEnumerable<MarketOutcome> Outcomes { get; init; } = new List<MarketOutcome>();
    }

    public class MarketOutcome : ValueObject
    {
        public OutcomeType Type { get; init; }
        public decimal Odds { get; init; }
    }

    public enum OutcomeType
    {
        Yes,
        No,
        Over,
        Under
    }
}
