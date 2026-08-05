using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using WaylaAI.Booking.Application.Interfaces;

namespace WaylaAI.Booking.Infrastructure.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaymentService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> ProcessPaymentAsync(string bookingId, string userId, decimal amount, CancellationToken cancellationToken = default)
    {
        // Get the current authorization token from the request context

        var request = new { BookingId = bookingId, Amount = amount };
        
        var response = await _httpClient.PostAsJsonAsync("/api/payments", request, cancellationToken);
        
        return response.IsSuccessStatusCode;
    }
}
