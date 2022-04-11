namespace MiniJob.Dapr.Http;

/// <summary>
/// Defines the methods for creating an HttpClient for direct access to the Dapr process endpoints.
/// For internal use only.
/// </summary>
public interface IDaprProcessHttpClientFactory
{
    /// <summary>
    /// Creates a <see cref="HttpClient"/> instance for invoking methods on the associated Dapr Process HTTP endpoints.
    /// </summary>
    /// <returns>A <see cref="HttpClient"/> instance.</returns>
    HttpClient CreateDaprHttpClient();
}
