using HotelBooking.Domain.ValueObjects;

namespace HotelBooking.Domain.Entities;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string GuestName { get; private set; } = null!;
    public BookingRange Range { get; private set; } = null!;
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Booking() { } // Required for EF Core

    public Booking(Guid id, Guid roomId, string guestName, BookingRange range, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(guestName))
        {
            throw new ArgumentException("Guest name cannot be empty.");
        }

        Id = id;
        RoomId = roomId;
        GuestName = guestName;
        Range = range;
        TotalPrice = range.DurationInDays * pricePerNight;
        CreatedAt = DateTime.UtcNow;
    }
}
