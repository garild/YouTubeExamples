namespace HealthCheck.Examples.Infrastructure.HealthChecks.Demo;

public sealed class DiskSpaceHealthCheckOptions
{
    public const string SectionName = "HealthChecks:DiskSpace";

    public string DriveName { get; set; } = "C";

    public long MinimumFreeMegabytes { get; set; } = 1024;
}
