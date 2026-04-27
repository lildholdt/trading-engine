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
    /// Creates a new sport event actor for the specified event registry entry. 
    /// If an actor for the given event ID already exists, the existing actor is retained.
    /// </summary>
    /// <param name="entry">The event registry item containing information about the sport event.</param>
    MatchId CreateAsync(RegistryItem entry);

    /// <summary>
    /// Stops the sport event actor associated with the specified event ID and releases any resources it holds. 
    /// This operation ensures that the actor is gracefully shut down.
    /// </summary>
    /// <param name="id">The unique identifier of the sport event whose actor should be stopped.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of stopping the actor.</returns>
    Task StopAsync(MatchId id);

    /// <summary>
    /// Resets the state of the system, clearing all existing sport event actors and their associated states.
    /// This method ensures the system is returned to a clean state, removing all active actors and their data.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of resetting the system.</returns>
    Task Reset();
}