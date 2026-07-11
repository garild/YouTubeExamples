using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace HealthCheck.Examples.HealthChecks;

public sealed class DiskSpaceHealthCheck(IOptions<DiskSpaceHealthCheckOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var drive = DriveInfo.GetDrives()
            .FirstOrDefault(d =>
                d.IsReady &&
                d.Name.StartsWith(settings.DriveName, StringComparison.OrdinalIgnoreCase));

        if (drive is null)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Drive '{settings.DriveName}' was not found or is not ready."));
        }

        var freeMegabytes = drive.AvailableFreeSpace / (1024 * 1024);
        var totalMegabytes = drive.TotalSize / (1024 * 1024);
        var data = new Dictionary<string, object>
        {
            ["drive"] = drive.Name,
            ["freeMegabytes"] = freeMegabytes,
            ["totalMegabytes"] = totalMegabytes,
            ["minimumFreeMegabytes"] = settings.MinimumFreeMegabytes
        };

        if (freeMegabytes < settings.MinimumFreeMegabytes)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Low disk space on {drive.Name}: {freeMegabytes} MB free.",
                data: data));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Disk space OK on {drive.Name}: {freeMegabytes} MB free.",
            data: data));
    }
}
