using System.Threading.Channels;
using TradingEngine.Domain.PlaceOrder;
using TradingEngine.Domain.UpdateOdds;
using TradingEngine.Infrastructure.CommandBus;

namespace TradingEngine.Domain;

public sealed class SportEventActor
{
    // Dependencies
    private readonly Channel<ISportEventMessage> _mailbox;
    private readonly ICommandBus _commandBus;
    private readonly IOrderStrategy _orderStrategy;
    private readonly IOddsProvider _oddsProvider;

    // State
    private SportEventId Id { get; init; }
    private Dictionary<string, Bookmaker> Bookmakers { get; set; } = new();
    private decimal LatestPrice { get; set; }

    public SportEventActor(
        SportEventId id,
        ICommandBus commandBus,
        IOrderStrategy orderStrategy,
        IOddsProvider oddsProvider)
    {
        Id = id;
        _commandBus = commandBus;
        _orderStrategy = orderStrategy;
        _oddsProvider = oddsProvider;
        
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
            try
            {
                await message.ApplyAsync(this);
            }
            catch (Exception ex)
            {
                // Log and handle the exception to keep the loop running
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
    
    private async Task PollOdds()
    {
        while (true)
        {
            var odds = await _oddsProvider.GetOdds(Id);
            if (odds == null) continue;

            await SendAsync(new UpdateOddsMessage
            {
                SportEventId = Id,
                Bookmakers = odds
            });

            // TODO: Apply dynamic polling which is adjusted when start time approaches
            await Task.Delay(100000);
        }
    }

    // --- state mutation helpers ---
    
    public async Task ApplyOddsUpdate(IEnumerable<Bookmaker> odds)
    {
        var price = _orderStrategy.CalculatePrice(odds);
        if (LatestPrice != price)
        {
            LatestPrice = price;
            await _commandBus.SendAsync(new PlaceOrderCommand
            {
                Id = Id,
                Price = price,
            });
        } 
    }
}
