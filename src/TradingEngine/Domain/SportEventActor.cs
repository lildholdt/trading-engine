using System.Threading.Channels;
using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;

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
}
