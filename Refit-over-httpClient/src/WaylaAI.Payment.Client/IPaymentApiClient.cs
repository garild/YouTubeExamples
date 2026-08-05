using Refit;

namespace WaylaAI.Payment.Client
{
    public sealed record ProcessPaymentRequest(string BookingId, decimal Amount);
    public interface IPaymentApiClient
    {
        [Post("/api/payments")]
        Task<ApiResponse<bool>> ProcessPaymentAsync([Body] ProcessPaymentRequest request, CancellationToken ct = default);
    }
}
