namespace Products.Api.Domain;

public record Product(
    Guid Id,
    string Name,
    decimal Price,
    string Sku,
    DateTime CreatedAt
);
