namespace WaylaAI.Payment.Api.Domain;

public sealed class Payment
{
    public Guid Id { get; private set; }
    public string BookingId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = "Pending";
    public DateTime CreatedAt { get; private set; }

    public static Payment Create(string bookingId, string userId, decimal amount)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            UserId = userId,
            Amount = amount,
            Status = "Completed", // We assume it's processed right away
            CreatedAt = DateTime.UtcNow
        };
    }
}
