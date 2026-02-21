using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Extensions.Options;

namespace TradingEngine.Utils;

public class DeterministicTeamMatcher(IOptions<ApplicationSettings> options) : ITeamMatcher
{
    private readonly ApplicationSettings _options = options.Value;
    
    // A collection of "noise words" that are considered irrelevant when normalizing team names.
    // These are common elements found in football team names, such as "fc", "club", or "u21".
    private static readonly HashSet<string> NoiseWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "fc", "cf", "sc", "afc", "bk", "fk", "if", // Common abbreviations in football team names
        "football", "club", "women", "w", "u19", "u21" // Additional descriptive words or age groups
    };

    // This method normalizes the input string by performing multiple transformations
    // including case normalization, diacritic removal, punctuation removal, and noise word filtering.
    private string Normalize(string input)
    {
        // If the input is null, empty, or whitespace, return an empty string.
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert the input to lowercase to ensure case-insensitivity.
        var value = input.ToLowerInvariant();

        // Remove diacritical marks (e.g., accents) from characters.
        value = RemoveDiacritics(value);

        // Replace any punctuation with spaces using a regular expression.
        // This ensures only alphanumeric characters and spaces remain.
        value = Regex.Replace(value, @"[^\w\s]", " ");

        // Normalize whitespace: replace multiple spaces with a single space and trim leading/trailing spaces.
        value = Regex.Replace(value, @"\s+", " ").Trim();

        // Split the string into individual words/tokens, remove any noise words,
        // and keep only the relevant parts of the name.
        var tokens = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split by spaces while removing empty entries.
            .Where(t => !NoiseWords.Contains(t)); // Exclude tokens that are in the NoiseWords set.

        // Rejoin the filtered tokens into a single string, separated by spaces.
        return string.Join(" ", tokens);
    }

    // This private helper method removes diacritical marks (e.g., accents) from characters in a string.
    private string RemoveDiacritics(string text)
    {
        // Normalize the text to a decomposed form (FormD), which separates base characters and diacritical marks.
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        // Iterate through each character in the decomposed string.
        foreach (var c in normalized)
        {
            // Check the Unicode category of the character.
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            
            // If the character is not a non-spacing mark (i.e., not a diacritical mark), add it to the StringBuilder.
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        // Return the string in its composed form (FormC) after removing diacritical marks.
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
    
    // This method calculates the Jaccard similarity coefficient between two strings.
    // The Jaccard similarity measures the similarity between two sets of tokens
    // as the size of their intersection divided by the size of their union.
    private double Jaccard(string a, string b)
    {
        // Split the first string into tokens (words) by whitespace and remove empty entries.
        // Convert the resulting tokens into a HashSet to ensure unique tokens and allow set operations.
        var setA = a.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        // Repeat the process for the second string.
        var setB = b.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        // Calculate the size of the intersection between the two sets.
        var intersection = setA.Intersect(setB).Count();

        // Calculate the size of the union between the two sets.
        var union = setA.Union(setB).Count();

        // If the union is zero (e.g., both strings are empty or contain no valid tokens),
        // return a similarity of 0, as there is no meaningful comparison to be made.
        if (union == 0) return 0;

        // Calculate and return the Jaccard similarity as the ratio of the intersection size
        // to the union size. Cast the result to a double to ensure fractional precision.
        return (double)intersection / union;
    }

    // This private method calculates the Jaro-Winkler similarity between two strings.
    // It uses the 'Fuzz.TokenSetRatio' method to determine the similarity score as a percentage.
    // The score is then normalized to a range between 0.0 and 1.0 by dividing the result by 100.
    private static double JaroWinkler(string a, string b)
    {
        return Fuzz.TokenSetRatio(a, b) / 100.0;
    }
    
    public double MatchScore(string nameA, string nameB)
    {
        // Step 1: Normalize both team names to remove noise words, punctuation, diacritics, etc.
        var normA = Normalize(nameA);
        var normB = Normalize(nameB);

        // Step 2: If either name is empty after normalization, return a score of 0 (no meaningful comparison).
        if (string.IsNullOrWhiteSpace(normA) || string.IsNullOrWhiteSpace(normB))
            return 0;

        // Step 3: Calculate token-based similarity using the Jaccard coefficient.
        var tokenScore = Jaccard(normA, normB);

        // Step 4: Calculate fuzzy similarity using the Jaro-Winkler algorithm.
        var fuzzyScore = JaroWinkler(normA, normB);

        // Step 5: Compute a weighted composite score by combining the token-based and fuzzy similarity scores.
        // The token-based score is weighted at 60%, and the fuzzy score is weighted at 40%.
        var finalScore =
            _options.TokenWeight * tokenScore + // Token similarity has more weight in the composite score.
            _options.FuzzyWeight * fuzzyScore;  // Fuzzy similarity contributes less but accounts for subtle differences.

        // Step 6: Return the final composite similarity score as a value between 0.0 and 1.0.
        return finalScore;
    }

    public bool IsMatch(string nameA, string nameB)
        => MatchScore(nameA, nameB) >= _options.Threshold; // Return true if the match score meets or exceeds the threshold.
}