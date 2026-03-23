using System.Threading.Channels;
using TradingEngine.Domain.Odds;
using TradingEngine.Domain.Odds.OddsUpdated;
using TradingEngine.Domain.Odds.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public sealed class SportEventActor
{
    // Dependencies
    private readonly Channel<ISportEventMessage> _mailbox;
    private readonly IEventBus _eventBus;
    private readonly IOddsProvider _oddsProvider;

    // State
    private SportEventId Id { get; init; }
    private List<Bookmaker> Odds { get; set; } = [];

    public SportEventActor(
        SportEventId id,
        IEventBus eventBus,
        IOddsProvider oddsProvider)
    {
        Id = id;
        _eventBus = eventBus;
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
            if (odds.Count == 0) continue;

            await SendAsync(new UpdateOddsMessage
            {
                SportEventId = Id,
                Bookmakers = odds
            });

            // TODO: Apply dynamic polling which is adjusted when start time approaches
            
            // x hours before start time
            // 3 days before 3 min. 
            // 2 days before 2 min.
            // 1 days before 1 min => decrease linearly to 5 sek.
            // 6 hours before 5 sek. 
            // Stop when 30 min to start time
            
            // Added closing line prices from Polymarket / OddsAPI (raw odds)
            await Task.Delay(1000);
        }
    }

    // --- state mutation helpers ---
    
    public async Task ApplyOddsUpdate(IReadOnlyCollection<Bookmaker> odds)
    {
        if (Equals(odds, Odds))
            return;
        
        // Find all bookmakers that have changed by comparing the old and new collections
        var changedBookmakers = odds
            .Where(newBookmaker => 
                !Odds.Any(existingBookmaker => existingBookmaker.Equals(newBookmaker)))
            .ToList();
        
        // Update only the changed bookmakers in the Bookmakers collection
        foreach (var changedBookmaker in changedBookmakers)
        {
            var existingBookmaker = Odds.FirstOrDefault(b => b.Name == changedBookmaker.Name);
            if (existingBookmaker != null)
            {
                // Replace the existing bookmaker with the new one
                Odds.Remove(existingBookmaker);
            }
            Odds.Add(changedBookmaker);
        }

        if (changedBookmakers.Count == 0)
            return;
        
        // Notify that odds has been updated
        await _eventBus.PublishAsync(new OddsUpdatedEvent
        {
            Id = Id,
            Odds = Odds
        });
    }
}
