using Microsoft.AspNetCore.SignalR;

namespace TradingEngine.Domain;

public class SportEventHub(ILogger<SportEventHub> logger) : Hub
{
    public async Task SendSportEventUpdate(string title, DateTime dateTime)
    {
        await Clients.All.SendAsync("SportEvent", title, dateTime);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("Connected", Context.ConnectionId);
        logger.LogInformation("Connected client {clientId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}