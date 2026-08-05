using HotelBooking.Application;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

// Seed data for demo purposes
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roomId = Guid.Parse("d128df67-5d7d-417d-8959-1e30a597a731");
    if (!context.Rooms.Any(r => r.Id == roomId))
    {
        context.Rooms.Add(new Room(roomId, "101", 150.00m));
        context.SaveChanges();
    }
}

app.Run();

// Required for integration testing using WebApplicationFactory
public partial class Program { }
