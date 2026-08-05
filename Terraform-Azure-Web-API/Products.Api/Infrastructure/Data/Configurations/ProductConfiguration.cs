using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Api.Domain;

namespace Products.Api.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Seed data for local Dev environment
        builder.HasData(
            new Product(Guid.Parse("7a0fd781-da04-4038-9095-7b677ae59861"), "Mechanical Keyboard", 129.99m, "KB-MECH-100", new DateTime(2026, 7, 10, 12, 0, 0, DateTimeKind.Utc)),
            new Product(Guid.Parse("7a0fd781-da04-4038-9095-7b677ae59862"), "Wireless Ergonomic Mouse", 79.50m, "MS-WIRE-200", new DateTime(2026, 7, 11, 14, 30, 0, DateTimeKind.Utc)),
            new Product(Guid.Parse("7a0fd781-da04-4038-9095-7b677ae59863"), "UltraWide 34 inch Monitor", 449.99m, "MN-UWD-300", new DateTime(2026, 7, 12, 9, 15, 0, DateTimeKind.Utc))
        );
    }
}
