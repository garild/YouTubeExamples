namespace WaylaAI.Booking.Application.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(string bookingId, string userId, decimal amount, CancellationToken cancellationToken = default);
}
