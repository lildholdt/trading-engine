using System.Threading.Channels;
using TradingEngine.Domain.Events.OddsUpdated;
using TradingEngine.Domain.Messages;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public sealed class SportEventActor
{
    // Dependencies
    private readonly Channel<ISportEventCommand> _mailbox;
    private readonly IEventBus _eventBus;
    private readonly IOddsProvider _oddsProvider;

    // State
    private SportEventId Id { get; init; }
    private DateTime StartTime { get; init; }
    private List<Bookmaker> Odds { get; set; } = [];
    private readonly CancellationTokenSource _cts = new();
    
    public SportEventActor(
        SportEventId id,
        DateTime startTime,
        IEventBus eventBus,
        IOddsProvider oddsProvider)
    {
        Id = id;
        StartTime = startTime;
        _eventBus = eventBus;
        _oddsProvider = oddsProvider;
        
        _mailbox = Channel.CreateUnbounded<ISportEventCommand>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        
        _ = ReadMessagesAsync(_cts.Token);
        _ = PollOddsAsync(_cts.Token);
    }
    
    public ValueTask SendMessageAsync(ISportEventCommand command)
        => _mailbox.Writer.WriteAsync(command);

    private async Task ReadMessagesAsync(CancellationToken ct)
    {
        await foreach (var message in _mailbox.Reader.ReadAllAsync(ct))
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
    
    private async Task PollOddsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var odds = await _oddsProvider.GetOdds(Id);
            if (odds.Count == 0) continue;

            await SendMessageAsync(new UpdateOddsMessage
            {
                SportEventId = Id,
                Bookmakers = odds
            });
            
            // Check if the match has ended
            var timeUntilStart = StartTime - DateTime.UtcNow;
            if (timeUntilStart.TotalMilliseconds < 0) break;
            
            // Calculate odds polling interval
            var delayMilliseconds = timeUntilStart.TotalDays switch
            {
                > 3 => TimeSpan.FromMinutes(5).Milliseconds, // More than 3 days: 5 minutes
                > 2 => TimeSpan.FromMinutes(3).Milliseconds, // 2-3 days: 3 minutes
                > 1 => TimeSpan.FromMinutes(2).Milliseconds, // 1-2 days: 2 minutes
                _ when timeUntilStart.TotalHours > 6 => TimeSpan.FromMinutes(1).Milliseconds, // 6-24 hours: 1 minute
                _ => TimeSpan.FromSeconds(5).Milliseconds // Less than 6 hours: 5 seconds
            };
            
            try
            {
                // Use Task.Delay with cancellation support
                await Task.Delay(delayMilliseconds, ct);
            }
            catch (TaskCanceledException)
            {
                // Task.Delay was canceled, exit the loop
                break;
            }
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
    
    public async Task EndMatch()
    {
        await _cts.CancelAsync();
    }
}
