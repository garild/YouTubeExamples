namespace HotelBooking.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string RoomNumber { get; private set; } = null!;
    public decimal PricePerNight { get; private set; }
    public bool IsActive { get; private set; }

    private Room() { } // Required for EF Core

    public Room(Guid id, string roomNumber, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
        {
            throw new ArgumentException("Room number cannot be empty.");
        }

        if (pricePerNight <= 0)
        {
            throw new ArgumentException("Price per night must be greater than zero.");
        }

        Id = id;
        RoomNumber = roomNumber;
        PricePerNight = pricePerNight;
        IsActive = true;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }
        PricePerNight = newPrice;
    }
}
