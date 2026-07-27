using MediatR;
using WaylaAI.Booking.Application.DTOs;

namespace WaylaAI.Booking.Application.UseCases.CreateBooking;

public sealed record CreateBookingCommand(
    string UserId,
    string Destination,
    DateTime Date,
    decimal Price
) : IRequest<BookingDto>;
