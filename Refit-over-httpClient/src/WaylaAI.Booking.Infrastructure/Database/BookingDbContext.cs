using Microsoft.EntityFrameworkCore;

namespace WaylaAI.Booking.Infrastructure.Database;

public sealed class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<WaylaAI.Booking.Domain.Entities.Booking> Bookings => Set<WaylaAI.Booking.Domain.Entities.Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<WaylaAI.Booking.Domain.Entities.Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
        });

        // Seed data
        var demoUserId = "auth0|6a57d35e5fc06ea27ce09865"; // We can replace this when we have the actual user ID or seed a known test user
        modelBuilder.Entity<WaylaAI.Booking.Domain.Entities.Booking>().HasData(
            new WaylaAI.Booking.Domain.Entities.Booking(Guid.Parse("d343467c-d6b3-4f9e-a868-233bb93efd68"), demoUserId, "Paris", new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1250.00m),
            new WaylaAI.Booking.Domain.Entities.Booking(Guid.Parse("b11c97f1-7c96-48cf-94f7-e435967bb1d9"), demoUserId, "Tokyo", new DateTime(2027, 3, 15, 0, 0, 0, DateTimeKind.Utc), 2800.00m),
            new WaylaAI.Booking.Domain.Entities.Booking(Guid.Parse("6789f2a9-c89b-466d-8c43-f6d8961726a7"), demoUserId, "New York", new DateTime(2027, 5, 20, 0, 0, 0, DateTimeKind.Utc), 900.50m)
        );
    }
}
