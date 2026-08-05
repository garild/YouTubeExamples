namespace WaylaAI.Booking.Domain.Entities;

public sealed class Booking
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public decimal Price { get; private set; }

    public Booking(Guid id, string userId, string destination, DateTime date, decimal price)
    {
        Id = id;
        UserId = userId;
        Destination = destination;
        Date = date;
        Price = price;
    }

    public static Booking Create(string userId, string destination, DateTime date, decimal price)
    {
        return new Booking(Guid.NewGuid(), userId, destination, date, price);
    }
}
