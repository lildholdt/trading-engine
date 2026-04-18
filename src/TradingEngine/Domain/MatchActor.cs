using System.Collections.ObjectModel;
using System.Threading.Channels;
using TradingEngine.Domain.StopMatch;
using TradingEngine.Domain.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain;

public sealed class MatchActor
{
    // Dependencies
    private readonly Channel<IMatchMessage> _mailbox;
    private readonly IEventBus _eventBus;
    private readonly IOddsProvider _oddsProvider;
    private readonly ILogger<MatchActor> _logger;

    // State
    private MatchId Id { get; }
    private string HomeTeam { get; }
    private string AwayTeam { get; }
    private DateTime StartTime { get; }
    private List<Bookmaker> Odds { get; set; } = [];
    
    private readonly CancellationTokenSource _cts = new();
    private readonly List<Task> _runningTasks = [];
    private bool Started => _runningTasks.Count != 0;
    
    public MatchActor(
        MatchId id,
        string homeTeam,
        string awayTeam,
        DateTime startTime,
        IEventBus eventBus,
        IOddsProvider oddsProvider,
        IServiceProvider serviceProvider)
    {
        Id = id;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        StartTime = startTime;
        _eventBus = eventBus;
        _oddsProvider = oddsProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<MatchActor>>();
        
        _mailbox = Channel.CreateUnbounded<IMatchMessage>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        
        _logger.LogInformation(
            "Actor created. Id={Id}, HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, StartTime={StartTime},",
            Id, HomeTeam, AwayTeam, StartTime
        );
    }

    public MatchState GetState() => new() {
        Id = Id,
        HomeTeam = HomeTeam,
        AwayTeam = AwayTeam,
        StartTime = StartTime,
        Odds = new ReadOnlyCollection<Bookmaker>(Odds)
    };
    
    public ValueTask SendMessageAsync(IMatchMessage message)
        => !Started ? throw new InvalidOperationException("Actor not started") : _mailbox.Writer.WriteAsync(message);
    
    public void StartAsync()
    {
        // Check if the actor is already started
        if (Started) 
            throw new InvalidOperationException("Actor already started");
        
        // Create cancellation token
        var ct = _cts.Token;
        
        // Start and track the polling task
        var pollingTask = Task.Run(() => PollOddsAsync(ct), ct);
        var mailboxTask = Task.Run(() => ReadMessagesAsync(ct), ct);
        
        // Add tasks to the running task list
        _runningTasks.Add(pollingTask);
        _runningTasks.Add(mailboxTask);
        
        _logger.LogInformation("Actor started for EventId: {EventId}", Id);
    }

    private async Task ReadMessagesAsync(CancellationToken ct)
    {
        try
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
                    _logger.LogError("Error processing message: {ExMessage}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Gracefully handle cancellation
            _logger.LogDebug("Message reading cancelled for eventId: {EventId}.", Id);
        }
        catch (Exception ex)
        {
            // Handle errors in the reading phase
            _logger.LogError("Error reading messages: {ExMessage}", ex.Message);
        }
    }
    
    private async Task PollOddsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting odds polling for EventId: {EventId}", Id);

        while (!ct.IsCancellationRequested)
        {
            // Check if the match has ended
            var timeUntilStart = StartTime - DateTime.UtcNow;
            if (timeUntilStart.TotalMilliseconds < 0) break;
                
            // Calculate odds polling interval
            var delayMilliseconds = timeUntilStart.TotalDays switch
            {
                > 3 => TimeSpan.FromMinutes(5).TotalMilliseconds, // More than 3 days: 5 minutes
                > 2 => TimeSpan.FromMinutes(3).TotalMilliseconds, // 2-3 days: 3 minutes
                > 1 => TimeSpan.FromMinutes(2).TotalMilliseconds, // 1-2 days: 2 minutes
                _ when timeUntilStart.TotalHours > 6 => TimeSpan.FromMinutes(1).TotalMilliseconds, // 6-24 hours: 1 minute
                _ => TimeSpan.FromSeconds(5).TotalMilliseconds // Less than 6 hours: 5 seconds
            };
            
            try
            {
                // Get new odds via the provider
                var odds = await _oddsProvider.GetOdds(Id).WaitAsync(ct);
                if (odds.Count == 0) continue;
                
                await SendMessageAsync(new UpdateOddsMessage
                {
                    MatchId = Id,
                    Bookmakers = odds
                });
                
                _logger.LogDebug("Next odds polling for EventId: {EventId} in {Delay} milliseconds.", Id, delayMilliseconds);
            
                // Use Task.Delay with cancellation support
                await Task.Delay((int)delayMilliseconds, ct);
            }
            catch (OperationCanceledException)
            {
                // Task.Delay was canceled, exit the loop gracefully
                _logger.LogDebug("Polling task cancelled for EventId: {EventId}", Id);
                return;
            }
            catch (Exception ex)
            {
                // Exception occured, log it and wait for next retry
                _logger.LogError(ex, "Error occurred while polling odds for EventId: {EventId}", Id);
                await Task.Delay((int)delayMilliseconds, ct);
            }
        }
    }

    // --- state mutation helpers ---
    
    public async Task ApplyOddsUpdate(IReadOnlyCollection<Bookmaker> odds)
    {
        // If the object are completely identical, do nothing.
        if (Equals(odds, Odds))
            return;

        // Flag to track if an update is needed
        var hasChanges = false;

        // Store the first odds update and notify
        if (Odds.Count == 0)
        {
            Odds = odds.ToList();
            hasChanges = true; // Mark changes to ensure the event is fired
        }
        else
        {
            // Find all bookmakers that have changed by comparing the old and new collections
            var changedBookmakers = odds
                .Where(newBookmaker => Odds.Any(existingBookmaker =>
                    existingBookmaker.Name == newBookmaker.Name && existingBookmaker.HasOutcomesChanged(newBookmaker)))
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

            // If any bookmakers have changed, mark the update
            if (changedBookmakers.Count > 0)
            {
                hasChanges = true;
            }

        }
        
        // If there are no changes, do nothing
        if (!hasChanges)
            return;

        // Notify that odds has been updated
        await _eventBus.PublishAsync(new OddsUpdatedEvent
        {
            Id = Id,
            Odds = Odds
        });
    }
    
    public async Task StopAsync()
    {
        if (!Started) 
            throw new InvalidOperationException("Must call StartAsync before Stop");

        // Stop accepting new messages and let the reader loop finish.
        _mailbox.Writer.TryComplete();
        
        // Signal cancellation
        await _cts.CancelAsync();

        // Notify that actor has been stopped
        await _eventBus.PublishAsync(new MatchStoppedEvent { Id = Id });
        
        _logger.LogInformation("Actor stopped for EventId: {EventId}", Id);
    }
}
