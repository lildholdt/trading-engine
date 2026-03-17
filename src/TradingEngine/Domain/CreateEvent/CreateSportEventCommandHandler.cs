using TradingEngine.Infrastructure.CommandBus;

namespace TradingEngine.Domain.CreateEvent;

public class CreateSportEventCommandHandler(ISportEventActorSystem actorSystem) :  ICommandHandler<CreateSportEventCommand>
{
    public Task HandleAsync(CreateSportEventCommand command, CancellationToken cancellationToken = default)
    {
        actorSystem.CreateAsync(command.Item);
        return Task.CompletedTask;
    }
}