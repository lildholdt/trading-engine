using System.Threading.Channels;
using TradingEngine.Domain.Matches.StopMatch;
using TradingEngine.Domain.Matches.UpdateOdds;
using TradingEngine.Infrastructure.EventBus;

namespace TradingEngine.Domain.Matches;

public sealed class MatchActor
{
    // Dependencies
    private readonly Channel<IMatchCommand> _mailbox;
    private readonly IEventBus _eventBus;
    private readonly IOddsProvider _oddsProvider;
    private readonly IMatchRepository _matchRepository;
    private readonly ILogger<MatchActor> _logger;

    // State
    private Match State { get; init; }
    
    private readonly CancellationTokenSource _cts = new();
    private readonly List<Task> _runningTasks = [];
    private bool Started => _runningTasks.Count != 0;
    
    public MatchActor(
        Match state,
        IEventBus eventBus,
        IOddsProvider oddsProvider,
        IMatchRepository matchRepository,
        IServiceProvider serviceProvider)
    {
        State = state;
        _eventBus = eventBus;
        _oddsProvider = oddsProvider;
        _matchRepository = matchRepository;
        _logger = serviceProvider.GetRequiredService<ILogger<MatchActor>>();
        
        _mailbox = Channel.CreateUnbounded<IMatchCommand>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        
        _logger.LogInformation(
            "Actor created. Id={Id}, HomeTeam={HomeTeam}, AwayTeam={AwayTeam}, StartTime={StartTime},",
            state.Id, state.HomeTeam, state.AwayTeam, state.StartTime
        );
    }
    
    public ValueTask SendMessageAsync(IMatchCommand command)
        => !Started ? throw new InvalidOperationException("Actor not started") : _mailbox.Writer.WriteAsync(command);
    
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
        
        _logger.LogInformation("Actor started for EventId: {EventId}", State.Id);
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
            _logger.LogDebug("Message reading cancelled for eventId: {EventId}.", State.Id);
        }
        catch (Exception ex)
        {
            // Handle errors in the reading phase
            _logger.LogError("Error reading messages: {ExMessage}", ex.Message);
        }
    }
    
    private async Task PollOddsAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting odds polling for EventId: {EventId}", State.Id);

        while (!ct.IsCancellationRequested)
        {
            // Check if the match has ended
            var timeUntilStart = State.StartTime - DateTime.UtcNow;
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

            if (State.IsPaused)
            {
                await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds, ct);
                continue;
            }
            
            try
            {
                // Get new odds via the provider
                var odds = await _oddsProvider.GetOdds(State.Id).WaitAsync(ct);
                if (odds.Count == 0) continue;
                
                await SendMessageAsync(new UpdateOddsCommand
                {
                    MatchId = State.Id,
                    Bookmakers = odds
                });
                
                _logger.LogDebug("Next odds polling for EventId: {EventId} in {Delay} milliseconds.", State.Id, delayMilliseconds);
            
                // Use Task.Delay with cancellation support
                await Task.Delay((int)delayMilliseconds, ct);
            }
            catch (OperationCanceledException)
            {
                // Task.Delay was canceled, exit the loop gracefully
                _logger.LogDebug("Polling task cancelled for EventId: {EventId}", State.Id);
                return;
            }
            catch (Exception ex)
            {
                // Exception occured, log it and wait for next retry
                _logger.LogError(ex, "Error occurred while polling odds for EventId: {EventId}", State.Id);
                await Task.Delay((int)delayMilliseconds, ct);
            }
        }
    }

    // --- state mutation helpers ---
    
    public async Task ApplyOddsUpdate(IReadOnlyCollection<Bookmaker> odds)
    {
        // If the object are completely identical, do nothing.
        if (Equals(odds, State.Odds))
            return;

        // Flag to track if an update is needed
        var hasChanges = false;

        // Store the first odds update and notify
        if (State.Odds.Count == 0)
        {
            State.Odds = odds.ToList();
            hasChanges = true; // Mark changes to ensure the event is fired
        }
        else
        {
            // Find all bookmakers that have changed by comparing the old and new collections
            var changedBookmakers = odds
                .Where(newBookmaker => State.Odds.Any(existingBookmaker =>
                    existingBookmaker.Name == newBookmaker.Name && existingBookmaker.HasOutcomesChanged(newBookmaker)))
                .ToList();

            // Update only the changed bookmakers in the Bookmakers collection
            foreach (var changedBookmaker in changedBookmakers)
            {
                var existingBookmaker = State.Odds.FirstOrDefault(b => b.Name == changedBookmaker.Name);
                if (existingBookmaker != null)
                {
                    // Replace the existing bookmaker with the new one
                    State.Odds.Remove(existingBookmaker);
                }

                State.Odds.Add(changedBookmaker);
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
        
        // Save current state
        await _matchRepository.SaveAsync(State);
        
        // Notify that odds has been updated
        await _eventBus.PublishAsync(new OddsUpdatedEvent
        {
            Match = State,
            UpdatedAtUtc = DateTime.UtcNow
        });
    }

    public async Task PauseAsync()
    {
        if (State.IsPaused)
            return;

        State.IsPaused = true;
        await _matchRepository.SaveAsync(State);
        _logger.LogInformation("Actor paused for EventId: {EventId}", State.Id);
    }

    public async Task ResumeAsync()
    {
        if (!State.IsPaused)
            return;

        State.IsPaused = false;
        await _matchRepository.SaveAsync(State);
        _logger.LogInformation("Actor resumed for EventId: {EventId}", State.Id);
    }

    public LiveMatchReadModel GetLiveReadModel()
    {
        var odds = State.Odds
            .Select(bookmaker => new LiveOddsReadModel(
                bookmaker.Name,
                bookmaker.Outcome.Home,
                bookmaker.Outcome.Away,
                bookmaker.Outcome.Draw,
                bookmaker.UpdatedAt))
            .ToList();

        return new LiveMatchReadModel(
            State.Id.Value,
            State.HomeTeam,
            State.AwayTeam,
            State.Series,
            State.StartTime,
            State.IsPaused,
            odds);
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
        await _eventBus.PublishAsync(new MatchStoppedEvent
        {
            Id = State.Id,
            StoppedAtUtc = DateTime.UtcNow
        });
        
        _logger.LogInformation("Actor stopped for EventId: {EventId}", State.Id);
    }
}
