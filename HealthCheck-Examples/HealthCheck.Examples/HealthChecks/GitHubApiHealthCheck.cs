using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Examples.HealthChecks;

public sealed class GitHubApiHealthCheck(HttpClient httpClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("zen", cancellationToken);

        var data = new Dictionary<string, object>
        {
            ["statusCode"] = (int)response.StatusCode
        };

        if (!response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Unhealthy(
                $"GitHub API returned {(int)response.StatusCode}.",
                data: data);
        }

        var zen = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
        data["zen"] = zen;

        return HealthCheckResult.Healthy("GitHub API is reachable.", data: data);
    }
}
