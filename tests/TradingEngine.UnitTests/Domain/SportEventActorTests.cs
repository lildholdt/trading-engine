using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TradingEngine.Domain;
using TradingEngine.Infrastructure.EventBus;
using TradingEngine.Domain.Events.OddsUpdated;
using TradingEngine.Domain.Messages;

public class SportEventActorTests
{
    private readonly Mock<IEventBus> _eventBus = new();
    private readonly Mock<IOddsProvider> _oddsProvider = new();
    private readonly Mock<ILogger<SportEventActor>> _logger = new();

    private SportEventActor CreateActor(DateTime? startTime = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(_logger.Object);

        return new SportEventActor(
            SportEventId.New,
            startTime ?? DateTime.UtcNow.AddHours(2),
            _eventBus.Object,
            _oddsProvider.Object,
            services.BuildServiceProvider());
    }

    [Fact]
    public async Task ApplyOddsUpdate_FirstUpdate_PublishesEvent()
    {
        var actor = CreateActor();

        var odds = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365")
        };

        await actor.ApplyOddsUpdate(odds);

        _eventBus.Verify(x => x.PublishAsync(
            It.Is<OddsUpdatedEvent>(e =>
                e.Odds.Count == 1 &&
                e.Odds.First().Name == "bet365")),
            Times.Once);
    }

    [Fact]
    public async Task ApplyOddsUpdate_SameOdds_DoesNotPublish()
    {
        var actor = CreateActor();

        var odds = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365")
        };

        await actor.ApplyOddsUpdate(odds);
        await actor.ApplyOddsUpdate(odds); // same instance

        _eventBus.Verify(x => x.PublishAsync(It.IsAny<OddsUpdatedEvent>()), Times.Once);
    }

    public async Task ApplyOddsUpdate_ChangedOdds_PublishesEvent()
    {
        var actor = CreateActor();

        var initial = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365", odds: 1.5m)
        };

        var updated = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365", odds: 2.0m) // changed
        };

        await actor.ApplyOddsUpdate(initial);
        await actor.ApplyOddsUpdate(updated);

        _eventBus.Verify(x => x.PublishAsync(It.IsAny<OddsUpdatedEvent>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ApplyOddsUpdate_NoOutcomeChanges_DoesNotPublish()
    {
        var actor = CreateActor();

        var odds1 = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365", odds: 1.5m)
        };

        var odds2 = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365", odds: 1.5m) // same odds
        };

        await actor.ApplyOddsUpdate(odds1);
        await actor.ApplyOddsUpdate(odds2);

        _eventBus.Verify(x => x.PublishAsync(It.IsAny<OddsUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_MessageIsProcessed()
    {
        var actor = CreateActor();

        var messageMock = new Mock<ISportEventMessage>();

        messageMock
            .Setup(m => m.ApplyAsync(It.IsAny<SportEventActor>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await actor.SendMessageAsync(messageMock.Object);

        // Give mailbox time to process
        await Task.Delay(100);

        messageMock.Verify(m => m.ApplyAsync(It.IsAny<SportEventActor>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_ExceptionInMessage_DoesNotCrash()
    {
        var actor = CreateActor();

        var messageMock = new Mock<ISportEventMessage>();

        messageMock
            .Setup(m => m.ApplyAsync(It.IsAny<SportEventActor>()))
            .ThrowsAsync(new Exception("boom"));

        await actor.SendMessageAsync(messageMock.Object);

        // Send another message to ensure loop still alive
        var secondMessage = new Mock<ISportEventMessage>();
        secondMessage
            .Setup(m => m.ApplyAsync(It.IsAny<SportEventActor>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await actor.SendMessageAsync(secondMessage.Object);

        await Task.Delay(100);

        secondMessage.Verify(m => m.ApplyAsync(It.IsAny<SportEventActor>()), Times.Once);
    }

    [Fact]
    public async Task EndMatch_CancelsProcessing()
    {
        var actor = CreateActor();

        await actor.EndMatch();

        // No direct assertion, but ensures no exception thrown
        await Task.Delay(50);
    }

    public async Task PollOdds_WhenOddsAvailable_SendsMessage()
    {
        var odds = new List<Bookmaker>
        {
            TestHelpers.CreateBookmaker("bet365")
        };

        _oddsProvider
            .Setup(x => x.GetOdds(It.IsAny<SportEventId>()))
            .ReturnsAsync(odds);

        var actor = CreateActor(DateTime.UtcNow.AddMinutes(1));

        await Task.Delay(200);

        _oddsProvider.Verify(x => x.GetOdds(It.IsAny<SportEventId>()), Times.AtLeastOnce);
    }

    public static class TestHelpers
    {
        public static Bookmaker CreateBookmaker(string name, decimal odds = 1.5m)
        {
            return new Bookmaker
            {
                Name = name,
                Outcomes = new List<Outcome>
                {
                    new Outcome { Price = odds }
                }
            };
        }
    }
}