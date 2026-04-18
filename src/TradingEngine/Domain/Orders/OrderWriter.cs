namespace TradingEngine.Domain.Matches.UpdateOdds;

/// <summary>
/// Defines a writer abstraction for persisting odds records.
/// </summary>
public interface IOddsWriter
{
    /// <summary>
    /// Persists a sequence of records.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="records">The records to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task WriteRecords<T>(IEnumerable<T> records, CancellationToken cancellationToken);
}

/// <summary>
/// CSV file implementation of <see cref="IOddsWriter"/>.
/// </summary>
public class OrderWriter(string filePath) : IOddsWriter
{
    /// <summary>
    /// Writes records to a CSV file.
    /// </summary>
    /// <typeparam name="T">The type of the records to write.</typeparam>
    /// <param name="records">The list of records.</param>
    /// <param name="cancellationToken"></param>
    public async Task WriteRecords<T>(IEnumerable<T> records, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (records == null)
        {
            throw new ArgumentException("Records collection cannot be null.", nameof(records));
        }

        var properties = typeof(T).GetProperties();

        // Check if the file exists
        var fileExists = File.Exists(filePath);

        // Open the file in append mode
        await using var writer = new StreamWriter(filePath, append: true);

        // Write the header only if the file does not already exist
        if (!fileExists)
        {
            await writer.WriteLineAsync(string.Join(",", properties.Select(p => $"\"{p.Name}\"")));
        }

        // Write each record
        foreach (var record in records)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(record)?.ToString();
                if (value != null)
                {
                    // Escape double quotes by doubling them
                    value = value.Replace("\"", "\"\"");
                    // Enclose the value in double quotes
                    return $"\"{value}\"";
                }
                return "\"\""; // Empty value
            });

            await writer.WriteLineAsync(string.Join(",", values));
        }
    }
}

