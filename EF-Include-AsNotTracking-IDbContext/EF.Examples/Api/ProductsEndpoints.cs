using EF.Examples.Application;
using EF.Examples.Application.Common;
using EF.Examples.Infrastructure.Persistence;

namespace EF.Examples.Api;

public static partial class ProductsEndpoints
{
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var result = await Handlers.GetProducts(dbContext, cancellationToken);
                return result.ToHttpResult();
            }).Produces<IReadOnlyList<ProductDto>>(StatusCodes.Status200OK);


        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var result = await Handlers.GetProductById(id, dbContext, cancellationToken);
            return result.ToHttpResult();
        }).Produces<ProductDto>(StatusCodes.Status200OK);

        return app;
    }
}
