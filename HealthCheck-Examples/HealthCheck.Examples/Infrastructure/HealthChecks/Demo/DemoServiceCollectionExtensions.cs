using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Examples.Infrastructure.HealthChecks.Demo;

public static class DemoServiceCollectionExtensions
{
    public static IServiceCollection AddExampleHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DiskSpaceHealthCheckOptions>(
            configuration.GetSection(DiskSpaceHealthCheckOptions.SectionName));

        services.AddHttpClient<GitHubApiHealthCheck>(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("HealthCheck-Examples/1.0");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        var healthChecks = services.AddHealthChecks()
            .AddCheck<DiskSpaceHealthCheck>("disk_space", tags: ["ready"])
            .AddCheck<GitHubApiHealthCheck>("github_api", tags: ["ready", "external"]);

        RegisterHttpBinCheck(services, healthChecks, configuration, "httpbin_200", "Healthy", ["ready", "demo", "httpbin"]);
        RegisterHttpBinCheck(services, healthChecks, configuration, "httpbin_500", "ServerError", ["ready", "demo", "httpbin"]);
        RegisterHttpBinCheck(services, healthChecks, configuration, "httpbin_timeout", "Timeout", ["ready", "demo", "httpbin"]);

        return services;
    }

    private static void RegisterHttpBinCheck(
        IServiceCollection services,
        IHealthChecksBuilder healthChecks,
        IConfiguration configuration,
        string checkName,
        string scenarioName,
        string[] tags)
    {
        var section = configuration.GetSection($"{HttpBinHealthCheckOptions.SectionName}:{scenarioName}");
        var options = section.Get<HttpBinHealthCheckOptions>()
            ?? throw new InvalidOperationException(
                $"Missing configuration section '{HttpBinHealthCheckOptions.SectionName}:{scenarioName}'.");

        var httpClientName = $"httpbin_{checkName}";

        services.AddHttpClient(httpClientName, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        healthChecks.Add(new HealthCheckRegistration(
            checkName,
            sp => new HttpBinHealthCheck(
                sp.GetRequiredService<IHttpClientFactory>(),
                httpClientName,
                options),
            failureStatus: null,
            tags));
    }
}
