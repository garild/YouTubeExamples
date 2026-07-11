using EF.Examples.Application;
using EF.Examples.Application.Common;
using EF.Examples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EF.Examples.Api;

public static partial class ProductsEndpoints
{
    public static class Handlers
    {
        public static Task<Result> CreateProduct(
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            //TODO
            return Task.FromResult(Result.Ok());
        }

        public static async Task<Result<ProductDto>> GetProductById(
            ApplicationDbContext dbContext,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var product = await dbContext.Products.FindAsync([id], cancellationToken);

            if (product is null)
                return Result<ProductDto>.NoData();

            return Result<ProductDto>.Ok(new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category?.FirstOrDefault()?.Name
            });
        }

        public static async Task<Result<IReadOnlyList<ProductDto>>> GetProducts(
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken = default)
        {
            var products = await dbContext.Products.AsNoTracking()
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category.FirstOrDefault().Name
                })
                .ToListAsync(cancellationToken);


            return Result<IReadOnlyList<ProductDto>>.Ok(products);
        }
    }
}
