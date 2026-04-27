using Microsoft.Extensions.Options;
using Moq;
using TradingEngine.Utils;
using Xunit;

namespace TradingEngine.UnitTests.Utils;

public class DeterministicTeamMatcherTests
{
    private readonly DeterministicTeamMatcher _teamMatcher;

    public DeterministicTeamMatcherTests()
    {
        // Mock configuration settings
        var mockOptions = new Mock<IOptions<ApplicationSettings>>();
        var teamMatching = new TeamMatching
        {
            Threshold = 0.8, // Matching threshold
            TokenWeight = 0.4, // Token similarity weight
            FuzzyWeight = 0.4, // Fuzzy similarity weight
            NoSpaceWeight = 0.2, // Fuzzy similarity weight
        };
        mockOptions.Setup(o => o.Value).Returns(new ApplicationSettings { TeamMatching = teamMatching });

        // Initialize the DeterministicTeamMatcher with the mocked options
        _teamMatcher = new DeterministicTeamMatcher(mockOptions.Object);
    }

    [Fact]
    public void MatchScore_ShouldReturnHighScore_ForSimilarNames()
    {
        // Arrange
        const string nameA = "FC Barcelona";
        const string nameB = "Barcelona Football Club";

        // Act
        var score = _teamMatcher.TeamScore(nameA, nameB);

        // Assert
        Assert.True(score > 0.9, $"Expected high similarity score, but got {score}");
    }

    [Fact]
    public void MatchScore_ShouldReturnZero_ForCompletelyDifferentNames()
    {
        // Arrange
        const string nameA = "Liverpool FC";
        const string nameB = "Real Madrid";

        // Act
        var score = _teamMatcher.TeamScore(nameA, nameB);

        // Assert
        Assert.True(score < 0.5);
    }

    [Fact]
    public void MatchScore_ShouldHandleNoiseWordsCorrectly()
    {
        // Arrange
        const string nameA = "FC Barcelona Women";
        const string nameB = "Barcelona";

        // Act
        var score = _teamMatcher.TeamScore(nameA, nameB);

        // Assert
        // Since "FC" and "Women" are noise words, the names should be reduced to "Barcelona".
        Assert.Equal(1.0, score);
    }

    [Fact]
    public void MatchScore_ShouldHandleEmptyOrWhitespaceInput()
    {
        // Arrange
        const string nameA = " ";
        const string nameB = "Liverpool FC";

        // Act
        var score = _teamMatcher.TeamScore(nameA, nameB);

        // Assert
        Assert.Equal(0, score);
    }

    [Fact]
    public void IsMatch_ShouldReturnTrue_ForHighSimilarity()
    {
        // Arrange
        const string nameA = "Manchester United";
        const string nameB = "Manchester United FC";

        // Act
        var isMatch = _teamMatcher.IsMatch(nameA, nameB);

        // Assert
        Assert.True(isMatch, "Expected the names to match based on similarity score.");
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_ForLowSimilarity()
    {
        // Arrange
        const string nameA = "Chelsea FC";
        const string nameB = "Arsenal";

        // Act
        var isMatch = _teamMatcher.IsMatch(nameA, nameB);

        // Assert
        Assert.False(isMatch, "Expected the names not to match due to low similarity score.");
    }

    [Theory]
    [InlineData("Juventus FC", "Juventus", true)] // Similar names, should match
    [InlineData("Paris Saint Germain", "PSG", false)] // Different abbreviations, may not match
    [InlineData("AC Milan", "Inter Milan", false)] // Different teams, should not match
    [InlineData("Man City", "Manchester City", true)] 
    [InlineData("Nottingham Fc Forest", "Nottingham Forest FC", true)] 
    [InlineData("Hamburg SV", "Hamburger SV", true)] 
    [InlineData("FC Phoenix 1976", "1976 FC Phoenix", true)] 
    [InlineData("Robert Morris Colonials", "Robert Morris", true)]
    [InlineData("Youngstown State Penguins", "Youngs town State Penguins", true)]
    public void IsMatch_ShouldReturnExpectedResult(string nameA, string nameB, bool expected)
    {
        // Act
        var isMatch = _teamMatcher.IsMatch(nameA, nameB);

        // Assert
        Assert.Equal(expected, isMatch);
    }
}