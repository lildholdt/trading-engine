namespace TradingEngine.Infrastructure.EventBus;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EventBus : IEventBus
{
    // A thread-safe dictionary to store event handlers for each event type
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    /// <summary>
    /// Publishes an event asynchronously to all its subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        var eventType = typeof(TEvent);

        // Check if there are any handlers for the event type
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var handlerTasks = handlers.Select(handler => handler(@event));
            await Task.WhenAll(handlerTasks); // Execute all handlers concurrently
        }
    }

    /// <summary>
    /// Subscribes an action to a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="action">The action to execute when the event is published.</param>
    public void Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var eventType = typeof(TEvent);

        // Add the handler to the list of handlers for the event type
        _handlers.AddOrUpdate(
            eventType,
            _ => new List<Func<object, Task>> { e => action((TEvent)e) },
            (_, existingHandlers) =>
            {
                existingHandlers.Add(e => action((TEvent)e));
                return existingHandlers;
            });
    }
}