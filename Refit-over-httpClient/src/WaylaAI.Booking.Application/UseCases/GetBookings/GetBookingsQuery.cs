using MediatR;
using WaylaAI.Booking.Application.DTOs;

namespace WaylaAI.Booking.Application.UseCases.GetBookings;

public sealed record GetBookingsQuery(string UserId) : IRequest<IEnumerable<BookingDto>>;
