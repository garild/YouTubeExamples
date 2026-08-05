param(
    [string]$ProjectPath = "$PSScriptRoot\..\HealthCheck.Examples\HealthCheck.Examples.csproj"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ProjectPath)) {
    throw "Project file not found: $ProjectPath"
}

$secrets = @{
    "ConnectionStrings:PostgreSql" = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres"
    "ConnectionStrings:PostgreSqlAppDb" = "Host=localhost;Port=5432;Database=appdb;Username=postgres;Password=postgres"
    "ConnectionStrings:Redis" = "localhost:6379"
    "ConnectionStrings:RabbitMq" = "amqp://guest:guest@localhost:5672/"
    "Keycloak:BaseUrl" = "http://localhost:8080"
    "Keycloak:HealthUrl" = "http://localhost:9000/health/ready"
    "Keycloak:AdminUsername" = "admin"
    "Keycloak:AdminPassword" = "admin"
}

foreach ($entry in $secrets.GetEnumerator()) {
    dotnet user-secrets set $entry.Key $entry.Value --project $ProjectPath | Out-Null
    Write-Host "Set $($entry.Key)"
}

Write-Host "User secrets configured for local Docker Compose infrastructure."
