using Microsoft.EntityFrameworkCore;
using WaylaAI.Payment.Api.Domain;

namespace WaylaAI.Payment.Api.Infrastructure;

public sealed class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.Payment> Payments => Set<Domain.Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Payment>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.BookingId).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.Amount).HasPrecision(18, 2);
        });
    }
}
