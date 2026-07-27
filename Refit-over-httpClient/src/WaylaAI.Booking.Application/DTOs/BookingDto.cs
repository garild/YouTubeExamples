namespace WaylaAI.Booking.Application.DTOs;

public sealed record BookingDto(
    Guid Id,
    string Destination,
    DateTime Date,
    decimal Price
);
