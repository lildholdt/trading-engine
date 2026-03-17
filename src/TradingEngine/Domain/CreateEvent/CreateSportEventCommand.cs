using TradingEngine.Infrastructure.CommandBus;
using TradingEngine.Services.Registry;

namespace TradingEngine.Domain.CreateEvent;

public class CreateSportEventCommand : ICommand
{
    public required EventRegistryItem Item { get; init; }
}