using MediatR;
using WaylaAI.Booking.Application.DTOs;
using WaylaAI.Booking.Application.Interfaces;
using WaylaAI.Payment.Client;

namespace WaylaAI.Booking.Application.UseCases.CreateBooking;

public sealed class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _repository;
    private readonly IPaymentApiClient _paymentApi;

    public CreateBookingHandler(IBookingRepository repository, IPaymentApiClient paymentApi)
    {
        _repository = repository;
        _paymentApi = paymentApi;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = WaylaAI.Booking.Domain.Entities.Booking.Create(
            request.UserId,
            request.Destination,
            request.Date,
            request.Price
        );

        await _repository.AddAsync(booking, cancellationToken);

        // Process payment
        var paymentResult = await _paymentApi.ProcessPaymentAsync(new ProcessPaymentRequest(booking.Id.ToString(), request.Price), cancellationToken);
        if (!paymentResult.IsSuccessStatusCode)
        {
            // Ideally we would have a saga/outbox pattern here, but for this simple example we can throw or just log
            throw new InvalidOperationException("Payment failed for the booking.");
        }

        return new BookingDto(booking.Id, booking.Destination, booking.Date, booking.Price);
    }
}
