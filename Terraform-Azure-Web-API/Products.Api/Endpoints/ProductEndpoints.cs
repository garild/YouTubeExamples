using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Products.Api.Domain;
using Products.Api.Infrastructure.Data;

namespace Products.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products");

        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Results.Ok(products);
        })
        .WithName("GetProducts").RequireAuthorization();

        group.MapPost("/", async (CreateProductRequest request, AppDbContext dbContext, IValidator<CreateProductRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var newProduct = new Product(
                Guid.NewGuid(),
                request.Name.Trim(),
                request.Price,
                request.Sku.Trim().ToUpperInvariant(),
                DateTime.UtcNow
            );

            dbContext.Products.Add(newProduct);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/products/{newProduct.Id}", newProduct);
        })
        .WithName("CreateProduct")
        .RequireAuthorization();
    }
}

public record CreateProductRequest(string Name, decimal Price, string Sku);
