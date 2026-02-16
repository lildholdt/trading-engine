using Microsoft.AspNetCore.SignalR;

namespace TradingEngine.Infrastructure.Hub;

public class GenericEventHub : Microsoft.AspNetCore.SignalR.Hub
{
    // You can add methods here if you need server-to-client communication directly.
}

public class HubPublisher<TEvent>(IHubContext<GenericEventHub> context) : IHubPublisher<TEvent> where TEvent : class
{
    private readonly IHubContext<GenericEventHub> _hubContext = context ?? throw new ArgumentNullException(nameof(context));

    // Publish an event to all connected clients.
    public async Task PublishAsync(TEvent @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        // Broadcast the event to all clients connected to the GenericEventHub.
        await _hubContext.Clients.All.SendAsync(typeof(TEvent).Name, @event);
    }
}