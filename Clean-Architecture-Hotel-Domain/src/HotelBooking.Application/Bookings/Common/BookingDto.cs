namespace HotelBooking.Application.Bookings.Common;

public record BookingDto(
    Guid Id,
    Guid RoomId,
    string GuestName,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice,
    DateTime CreatedAt
);
