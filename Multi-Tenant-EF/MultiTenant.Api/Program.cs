using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Api;
using MultiTenant.Api.Enitites;
using MultiTenant.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(p =>
{
    p.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddHostedService<DatabaseMigrationHostedService>();

builder.Services.AddScoped<TenantResolutionMiddleware>();
builder.Services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();

builder.Host.ConfigureHostOptions(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ServicesStartConcurrently = true;
});
builder.Services.AddDbContext<MultiTenantDbContext>();

var app = builder.Build();

//Add Midleware to resolve the tenant from the request
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseHttpsRedirection();


app.MapGet("/", async (MultiTenantDbContext dbContext) =>
{
    var aa = dbContext.Orders.ToArray();
    return Results.Ok();
});
app.MapPost("/orders", async ([FromBody] OrderDto orderDto, MultiTenantDbContext dbContext) =>
{
    var order = new Order
    {
        Name = orderDto.Name,
        CompanyName = orderDto.CompanyName,
        CreatedAt = DateTimeOffset.UtcNow
    };
    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/orders", async (MultiTenantDbContext dbContext) =>
{
    var orders = await dbContext.Orders.AsTracking().Select(o => new OrderDto(o.Name, o.CompanyName, o.CreatedAt)).Take(10).ToListAsync();
    return Results.Ok(orders);
});
await app.RunAsync();


public record OrderDto(string Name, string CompanyName, DateTimeOffset CreatedAt);