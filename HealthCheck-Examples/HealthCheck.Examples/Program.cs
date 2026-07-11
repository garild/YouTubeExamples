using HealthCheck.Examples.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExampleHealthChecks(builder.Configuration);
builder.Services
    .AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/health-ui"));

app.MapHealthChecks("/health", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.AddCustomStylesheet("dotnet.css");
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.Run();
