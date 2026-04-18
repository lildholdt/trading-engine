using TradingEngine.Domain.Matches;
using Xunit;

namespace TradingEngine.UnitTests.Domain
{
    public class BookmakerTests
    {
        [Fact]
        public void CalculateTrueOdds_SingleOutcomeType_CalculatesCorrectly()
        {
            var outcome = new Outcome
            {
                Home = 3.58m,
                Away = 2.25m,
                Draw = 2.92m
            };

            var trueOddsHome = outcome.CalculateTrueOdds(OutcomeType.Home);

            Assert.Equal(3.36m, trueOddsHome);
        }

        [Fact]
        public void CalculateTrueOdds_AllOutcomes_CalculatesCorrectly()
        {
            var outcome = new Outcome
            {
                Home = 3.58m,
                Away = 2.25m,
                Draw = 2.92m
            };

            var trueOdds = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, outcome.CalculateTrueOdds(OutcomeType.Home) },
                { OutcomeType.Away, outcome.CalculateTrueOdds(OutcomeType.Away) },
                { OutcomeType.Draw, outcome.CalculateTrueOdds(OutcomeType.Draw) }
            };

            var expected = new Dictionary<OutcomeType, decimal>
            {
                { OutcomeType.Home, 3.36m },
                { OutcomeType.Away, 2.11m },
                { OutcomeType.Draw, 2.74m }
            };

            Assert.Equal(expected, trueOdds);
        }
        
        [Fact]
        public void HasOutcomesChanged_WithSameData_ReturnsFalse()
        {
            var outcome = new Outcome
            {
                Home = 2.5m,
                Away = 3.0m,
                Draw = 3.5m
            };

            var bookmaker1 = new Bookmaker("TestBookmaker", outcome);
            var bookmaker2 = new Bookmaker("TestBookmaker", outcome);

            Assert.False(bookmaker1.HasOutcomesChanged(bookmaker2));
        }

        [Fact]
        public void HasOutcomesChanged_WithDifferentOutcomes_ReturnsTrue()
        {
            var bookmaker1 = new Bookmaker("TestBookmaker", new Outcome
            {
                Home = 2.5m,
                Away = 3.0m,
                Draw = 3.5m
            });

            var bookmaker2 = new Bookmaker("TestBookmaker", new Outcome
            {
                Home = 2.6m,
                Away = 3.0m,
                Draw = 3.5m
            });

            Assert.True(bookmaker1.HasOutcomesChanged(bookmaker2));
        }
    }
}