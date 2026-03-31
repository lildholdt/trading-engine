using System.Collections.Immutable;
using TradingEngine.Domain;
using Xunit;

namespace TradingEngine.UnitTests.Domain
{
    public class BookmakerTests
    {
        [Fact]
        public void TrueOdds_SingleOutcomeType_CalculatesCorrectly()
        {
            // Arrange
            var outcomes = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 3.58m },
                { OutcomeType.Away, 2.25m },
                { OutcomeType.Draw, 2.92m }
            }.ToImmutableDictionary();
            
            var bookmaker = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = DateTime.UtcNow,
                Outcomes = outcomes
            };

            // Act
            var trueOddsHome = bookmaker.TrueOdds(OutcomeType.Home);

            // Assert
            Assert.Equal(3.89m, trueOddsHome);
        }

        [Fact]
        public void TrueOdds_AllOutcomes_CalculatesCorrectly()
        {
            // Arrange
            var outcomes = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 3.58m },
                { OutcomeType.Away, 2.25m },
                { OutcomeType.Draw, 2.92m }
            }.ToImmutableDictionary();
            
            var bookmaker = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = DateTime.UtcNow,
                Outcomes = outcomes
            };

            // Act
            var trueOdds = bookmaker.TrueOdds();

            // Calculate expected True Odds for all outcomes
            var expected = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 3.89m },
                { OutcomeType.Away, 2.37m },
                { OutcomeType.Draw, 3.12m }
            };

            // Assert
            Assert.Equal(expected, trueOdds);
        }
        
        [Fact]
        public void IdenticalBookmakerObjects_ShouldBeEqual()
        {
            // Arrange: Create two identical Bookmaker objects
            var outcomes = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 2.5m },
                { OutcomeType.Away, 3.0m },
                { OutcomeType.Draw, 3.5m }
            }.ToImmutableDictionary();

            var bookmaker1 = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = new DateTime(2023, 1, 1),
                Outcomes = outcomes
            };

            var bookmaker2 = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = new DateTime(2023, 1, 2),
                Outcomes = new Dictionary<OutcomeType, decimal>(outcomes).ToImmutableDictionary() // Create a separate instance of the dictionary
            };

            // Act and Assert: Verify that the two objects are equal
            Assert.Equal(bookmaker1, bookmaker2); // Uses ValueObject equality
        }

        [Fact]
        public void DifferentBookmakerObjects_ShouldNotBeEqual()
        {
            // Arrange: Create two different Bookmaker objects
            var outcomes1 = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 2.5m },
                { OutcomeType.Away, 3.0m },
                { OutcomeType.Draw, 3.5m }
            }.ToImmutableDictionary();

            var outcomes2 = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 2.6m }, // Different value for Home outcome
                { OutcomeType.Away, 3.0m },
                { OutcomeType.Draw, 3.5m }
            }.ToImmutableDictionary();

            var bookmaker1 = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = new DateTime(2023, 1, 1),
                Outcomes = outcomes1
            };

            var bookmaker2 = new Bookmaker
            {
                Name = "TestBookmaker",
                LastUpdate = new DateTime(2023, 1, 1),
                Outcomes = outcomes2
            };

            // Act and Assert: Verify that the two objects are not equal
            Assert.NotEqual(bookmaker1, bookmaker2); // Uses ValueObject inequality
        }
    }
}