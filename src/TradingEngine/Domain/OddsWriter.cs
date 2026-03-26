namespace TradingEngine.Domain;

public interface IOddsWriter
{
    Task WriteRecords<T>(IEnumerable<T> records, CancellationToken cancellationToken);
}

public class OddsWriter(string filePath) : IOddsWriter
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

        if (records == null || !records.Any())
        {
            throw new ArgumentException("Records collection cannot be null or empty.", nameof(records));
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

