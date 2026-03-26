using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradingEngine.Clients.Polymarket;

public class JsonStringListConverter : JsonConverter<IReadOnlyCollection<string>>
{
    public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<List<string>>(reader.GetString() ?? "[]");
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyCollection<string> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(JsonSerializer.Serialize(value));
    }
}

public class JsonDecimalListConverter : JsonConverter<IReadOnlyCollection<decimal>>
{
    public override IReadOnlyCollection<decimal>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON string
        var jsonString = reader.GetString();
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return [];
        }

        // Deserialize the JSON string into a List<string>
        var stringList = JsonSerializer.Deserialize<List<string>>(jsonString);

        // Convert the List<string> to IReadOnlyCollection<decimal>
        var decimalList = new List<decimal>();
        foreach (var str in stringList!)
        {
            if (decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
            {
                decimalList.Add(decimalValue);
            }
            else
            {
                throw new JsonException($"Unable to convert '{str}' to a decimal.");
            }
        }

        return decimalList.AsReadOnly();
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyCollection<decimal> value, JsonSerializerOptions options)
    {
        // Convert the IReadOnlyCollection<decimal> to a List<string>
        var stringList = new List<string>();
        foreach (var decimalValue in value)
        {
            stringList.Add(decimalValue.ToString(CultureInfo.InvariantCulture));
        }

        // Serialize the List<string> as a JSON string
        writer.WriteStringValue(JsonSerializer.Serialize(stringList));
    }
}