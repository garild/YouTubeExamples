using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Examples.HealthChecks;

public sealed class HttpBinHealthCheck(
    IHttpClientFactory httpClientFactory,
    string httpClientName,
    HttpBinHealthCheckOptions options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(httpClientName);
        var data = new Dictionary<string, object>
        {
            ["url"] = options.Url,
            ["timeoutSeconds"] = options.TimeoutSeconds
        };

        try
        {
            using var response = await client.GetAsync(options.Url, cancellationToken);
            data["statusCode"] = (int)response.StatusCode;

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy(
                    $"HTTP check succeeded with {(int)response.StatusCode}.",
                    data: data)
                : HealthCheckResult.Unhealthy(
                    $"HTTP check failed with {(int)response.StatusCode}.",
                    data: data);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy(
                $"HTTP check timed out after {options.TimeoutSeconds} seconds.",
                data: data);
        }
    }
}
