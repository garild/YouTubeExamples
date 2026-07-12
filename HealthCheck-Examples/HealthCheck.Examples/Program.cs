using HealthCheck.Examples.Infrastructure;
using HealthCheck.Examples.Infrastructure.HealthChecks;
using HealthChecks.UI.Client;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
//builder.Services.AddExampleHealthChecks(builder.Configuration);

var options = new SmartHealthCheckOptions();
builder.Services.Configure<SmartHealthCheckOptions>(
            builder.Configuration.GetSection(SmartHealthCheckOptions.SectionName));

builder.Services.AddOptionsWithValidateOnStart<SmartHealthCheckOptions>().ValidateDataAnnotations();

builder.Configuration.GetSection(SmartHealthCheckOptions.SectionName).Bind(options);

builder.Services.AddSmartHealthChecks(options);

builder.Services
    .AddHealthChecksUI()
    .AddSqliteStorage("Data Source=healthchecks.db");

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/health-ui"));

app.MapHealthChecks("/health", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("readiness"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new()
{
    Predicate = check => check.Tags.Contains("liveness"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.AddCustomStylesheet("dotnet.css");
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.Run();
