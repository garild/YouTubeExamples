namespace MultiTenant.Api.Enitites
{
    public class Order
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string CompanyName { get; init; }
        public required DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
