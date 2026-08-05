using MediatR;
using WaylaAI.Booking.Application.DTOs;
using WaylaAI.Booking.Application.Interfaces;

namespace WaylaAI.Booking.Application.UseCases.GetBookings;

public sealed class GetBookingsHandler : IRequestHandler<GetBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _repository;

    public GetBookingsHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        return bookings.Select(b => new BookingDto(b.Id, b.Destination, b.Date, b.Price));
    }
}
