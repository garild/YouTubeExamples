namespace HealthCheck.Examples.HealthChecks;

public sealed class HttpBinHealthCheckOptions
{
    public const string SectionName = "HealthChecks:HttpBin";

    public string Url { get; set; } = "https://httpbin.org/status/200";

    public int TimeoutSeconds { get; set; } = 3;
}
