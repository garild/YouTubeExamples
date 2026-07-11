using EF.Examples.Domain;
using Microsoft.EntityFrameworkCore;

namespace EF.Examples.Infrastructure.Persistence;

public static class InMemoryDbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        var supplier = new Supplier
        {
            CompanyName = "Acme Supplies",
            Email = "orders@acme.example",
            Phone = "+1-555-0100"
        };

        var product = new Product
        {
            Name = "Wireless Mouse",
            Price = 29.99m,
            Supplier = supplier,
            Images = [new ProductImage { Url = "https://cdn.example/mouse.png", IsPrimary = true }],
            Reviews = [new Review { Rating = 5, Comment = "Great product!" }],
            Tags = [new ProductTag { Name = "electronics" }, new ProductTag { Name = "peripherals" }]
        };

        product.AddCategory("Accessories");

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
