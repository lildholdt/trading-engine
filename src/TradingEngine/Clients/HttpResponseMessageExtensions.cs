using System.Text.Json;

namespace TradingEngine.Clients;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Deserializes the content of an HttpResponseMessage to a specified type using the provided JsonSerializerOptions.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="response">The HttpResponseMessage.</param>
    /// <param name="options">The JsonSerializerOptions to use. If null, default options are used.</param>
    /// <returns>The deserialized object of type T, or null if deserialization fails.</returns>
    public static async Task<T?> DeserializeJsonAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null)
    {
        if (response == null)
        {
            throw new ArgumentNullException(nameof(response), "HttpResponseMessage cannot be null.");
        }

        options ??= DefaultOptions;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, options);
    }
}