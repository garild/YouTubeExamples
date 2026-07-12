using HealthCheck.Examples.Infrastructure.HealthChecks.Demo;
using HealthChecks.System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace HealthCheck.Examples.Infrastructure.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        extension(IServiceCollection sevices)
        {
            public void AddSmartHealthChecks(SmartHealthCheckOptions options)
            {
                var builder = sevices.AddHealthChecks();

                if (options.Redis.Enabled)
                    builder.AddRedis(options.Redis.ConnectionString, name: "Redis", tags: [ "redis", "cache", "liveness"]);

                if (options.RabbitMq.Enabled)
                {
                    builder.Services.AddSingleton(sp =>
                    {
                        var factory = new ConnectionFactory
                        {
                            HostName = "localhost",
                            UserName = "guest",
                            Password = "guest"
                        };

                        // Creates the connection (Note: Use CreateConnection() if using older 6.x client versions)
                        return factory.CreateConnectionAsync().GetAwaiter().GetResult();

                    });

                    builder.AddRabbitMQ(name: "RabbitMQ", tags: ["rabbitmq", "messaging", "liveness"]);
                }

                if(options.PostgreSql.Enabled)
                    builder.AddNpgSql(options.PostgreSql.ConnectionString, name: "PostgreSQL", tags: ["postgresql", "database", "liveness"]);


                builder.AddDiskStorageHealthCheck(p=>
                {
                    p.AddDrive("C", 160000);

                }, "DiskStorage", HealthStatus.Degraded, tags: ["memory", "disk storage", "readiness"]);


                builder.AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["self", "readiness"]);

                if (options.Keycloak.Enabled)
                {
                    const string keycloakHttpClientName = "KeycloakHttpClient";

                    sevices.AddHttpClient(keycloakHttpClientName, client =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(options.Keycloak.TimeoutSeconds);
                        client.BaseAddress = new Uri(options.Keycloak.BaseUrl);
                    });

                    builder.Add(new HealthCheckRegistration(
                        "Keycloak",
                        sp => new KeycloakHealthCheck(
                            sp.GetRequiredService<IHttpClientFactory>(),
                            keycloakHttpClientName,
                            options.Keycloak),
                        failureStatus: HealthStatus.Unhealthy,
                        tags: ["self", "liveness"]));
                }

            }
        }
    }

    public sealed class KeycloakHealthCheck(
    IHttpClientFactory httpClientFactory,
    string httpClientName,
    KeycloakHealthCheckOptions options) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = httpClientFactory.CreateClient(httpClientName);

                var response = await client.GetAsync(options.HealthPath, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy("Keycloak is healthy.");


                return HealthCheckResult.Unhealthy($"Keycloak returned {(int)response.StatusCode}.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Keycloak is unhealth {ex.Message}.");
            }

        }
    }
}
