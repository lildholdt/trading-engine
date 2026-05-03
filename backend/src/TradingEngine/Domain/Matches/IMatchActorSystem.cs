using TradingEngine.Domain.Registry;

namespace TradingEngine.Domain.Matches;

/// <summary>
/// Defines the contract for a system that manages sport event actors. 
/// The system is responsible for creating, interacting with, and stopping actors that represent individual sport events.
/// </summary>
public interface IMatchActorSystem
{
    /// <summary>
    /// Sends a message to the appropriate actor for processing. 
    /// If no actor exists for the event in the message, the message will either be discarded or cause an error, depending on the implementation.
    /// </summary>
    /// <param name="command">The message to be sent to a sport event actor.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask SendAsync(IMatchCommand command);

    /// <summary>
    /// Gets live snapshots for all currently active match actors.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A read-only collection of live actor snapshots.</returns>
    Task<IReadOnlyCollection<Match>> GetAllLiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a live snapshot for a specific active match actor.
    /// </summary>
    /// <param name="id">The unique identifier of the target match actor.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The live actor snapshot if found; otherwise null.</returns>
    Task<Match?> GetByIdAsync(MatchId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new sport event actor for the specified event registry entry. 
    /// If an actor for the given event ID already exists, the existing actor is retained.
    /// </summary>
    /// <param name="entry">The event registry item containing information about the sport event.</param>
    Task<MatchId> CreateAsync(RegistryItem entry);

    /// <summary>
    /// Stops the sport event actor associated with the specified event ID and releases any resources it holds. 
    /// This operation ensures that the actor is gracefully shut down.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event whose actor should be stopped.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of stopping the actor.</returns>
    Task StopAsync(MatchId id);

    /// <summary>
    /// Pauses odds polling for the specified match actor.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event actor to pause.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task PauseAsync(MatchId id);

    /// <summary>
    /// Resumes odds polling for the specified paused match actor.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event actor to resume.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task ResumeAsync(MatchId id);

    /// <summary>
    /// Resets the state of the system, clearing all existing sport event actors and their associated states.
    /// This method ensures the system is returned to a clean state, removing all active actors and their data.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of resetting the system.</returns>
    Task Reset();
}