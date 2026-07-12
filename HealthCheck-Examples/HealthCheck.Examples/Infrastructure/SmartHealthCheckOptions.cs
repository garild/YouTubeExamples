using static HealthCheck.Examples.Infrastructure.RabbitMqHealthCheckOptions;

namespace HealthCheck.Examples.Infrastructure;

public sealed class SmartHealthCheckOptions
{
    public const string SectionName = "HealthChecks";

    public ServiceHealthCheckOptions PostgreSql { get; set; }

    public ServiceHealthCheckOptions Redis { get; set; }

    public RabbitMqHealthCheckOptions RabbitMq { get; set; }

    public KeycloakHealthCheckOptions Keycloak { get; set; }
}

public class ServiceHealthCheckOptions
{
    public bool Enabled { get; set; }

    public int TimeoutSeconds { get; set; } = 5;

    public required string ConnectionString { get; set; }
}

public sealed class RabbitMqHealthCheckOptions : ServiceHealthCheckOptions
{
    public required string ManagementUrl { get; set; }
}

public sealed class KeycloakHealthCheckOptions
{
    public required string BaseUrl { get; set; }
    public required string HealthPath { get; set; }
    public bool Enabled { get; set; }

    public int TimeoutSeconds { get; set; } = 5;
}
