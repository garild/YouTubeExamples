using EF.Examples.Api;
using EF.Examples.Infrastructure;
using EF.Examples.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Don't use on production, this is just for demo purposes to seed the in-memory database with sample data.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await InMemoryDbInitializer.InitializeAsync(dbContext);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapProductsEndpoints();

app.Run();
