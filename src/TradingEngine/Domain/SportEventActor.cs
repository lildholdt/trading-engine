using System.Text.Json;
using System.Threading.Channels;
using TradingEngine.Clients.OddsApi;
using TradingEngine.Clients.OddsApi.Models;
using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Domain.UpdateOdds;
using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain;

public sealed class SportEventActor
{
    // Dependencies
    private readonly Channel<ISportEventMessage> _mailbox;
    private readonly ICommandBus _commandBus;
    private readonly IOddsApiClient _oddsClient;
    private readonly IEventBus _eventBus;
    private readonly IOrderStrategy _orderStrategy;

    // State
    private RegistryItem RegistryItem { get; set; }
    private IEnumerable<Market> Markets { get; init; } = new List<Market>();

    public SportEventActor(
        RegistryItem registryItem,
        ICommandBus commandBus,
        IEventBus eventBus,
        IOrderStrategy orderStrategy,
        IOddsApiClient oddsClient)
    {
        RegistryItem = registryItem ??  throw new ArgumentNullException(nameof(registryItem));;
        _commandBus = commandBus;
        _eventBus = eventBus;
        _orderStrategy = orderStrategy;
        _oddsClient = oddsClient;
     
        if (registryItem.OddsApiEvent == null)
            throw new ArgumentException($"{nameof(registryItem.OddsApiEvent)} must not be null");
        
        _mailbox = Channel.CreateUnbounded<ISportEventMessage>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        _ = RunAsync();
        _ = PollOdds();
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
    
    private async Task PollOdds()
    {
        while (true)
        {
            var odds = await _oddsClient.GetOdds(RegistryItem.OddsApiEvent!.Id);
            if (odds == null) continue;

            await SendAsync(new UpdateOddsMessage
            {
                SportEventId = RegistryItem.Id,
                Odds = odds
            });

            // TODO: Apply dynamic polling which is adjusted when start time approaches
            await Task.Delay(1000);
        }
    }

    // --- state mutation helpers ---

    public async Task ApplyOddsUpdate(Odds odds)
    {
        var price = _orderStrategy.CalculatePrice(odds);
        await _commandBus.SendAsync(new PlaceOrderCommand
        {
            Id = RegistryItem.Id,
            Price = price,
        });
    }
}
