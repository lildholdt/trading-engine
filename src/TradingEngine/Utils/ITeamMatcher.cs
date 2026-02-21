namespace TradingEngine.Utils;

public interface ITeamMatcher
{
    // This method calculates a similarity score between two team names.
    // Parameters:
    // - nameA: The first team name to compare.
    // - nameB: The second team name to compare.
    // Returns:
    // - A double value representing the similarity score between the two team names.
    //   The score ranges from 0.0 (completely different) to 1.0 (perfect match).
    double MatchScore(string nameA, string nameB);
    
    // This method determines whether two team names are considered a match based on a specified similarity threshold.
    // Parameters:
    // - nameA: The first team name to compare.
    // - nameB: The second team name to compare.
    // Returns:
    // - A boolean value: true if the similarity score between the two names meets or exceeds the threshold, otherwise false.
    bool IsMatch(string nameA, string nameB);
}