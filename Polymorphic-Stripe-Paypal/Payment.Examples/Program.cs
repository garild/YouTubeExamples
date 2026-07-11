using Payment.Examples.Api;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonSerializerOptions>(p =>
{
    p.PropertyNameCaseInsensitive = true;
    p.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPaymentEndpoints();

app.Run();
