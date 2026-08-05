using HotelBooking.Application.Bookings.Common;
using MediatR;

namespace HotelBooking.Application.Bookings.Commands;

public record BookRoomCommand(
    Guid RoomId,
    string GuestName,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<BookingDto>;
