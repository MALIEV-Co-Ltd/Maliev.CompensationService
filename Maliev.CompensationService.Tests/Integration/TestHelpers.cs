using System.Net.Http.Json;
using System.Text.Json;

namespace Maliev.CompensationService.Tests.Integration;

/// <summary>
/// Helper methods for integration tests
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// JSON serializer options that match the API configuration (snake_case)
    /// </summary>
    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    /// Posts JSON content using snake_case serialization
    /// </summary>
    public static Task<HttpResponseMessage> PostAsJsonSnakeCaseAsync<T>(
        this HttpClient client,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(requestUri, value, JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Puts JSON content using snake_case serialization
    /// </summary>
    public static Task<HttpResponseMessage> PutAsJsonSnakeCaseAsync<T>(
        this HttpClient client,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default)
    {
        return client.PutAsJsonAsync(requestUri, value, JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Reads JSON content using snake_case deserialization
    /// </summary>
    public static Task<T?> ReadFromJsonSnakeCaseAsync<T>(
        this HttpContent content,
        CancellationToken cancellationToken = default)
    {
        return content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }
}
