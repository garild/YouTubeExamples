namespace EF.Examples.Application
{
    public record ProductDto
    {
        public required string Name { get; init; }

        public decimal Price { get; init; }

        public string? Category { get; init; }
    }
}
