var request = new CreateBookingRequest("Paris", 1500, "EUR");

var money = new Money(
    request.Amount,
    request.Currency);

var booking = new Booking(
    Guid.NewGuid(),
    request.Destination,
    money); 

Console.WriteLine($"""
Booking Created:
Destination: {booking.Destination}
Price: {booking.Price.Amount} {booking.Price.Currency}
""");

public class Booking
{
    public Guid Id { get; private set; }

    public string Destination { get; private set; }

    public Money Price { get; private set; }

    public Booking(Guid id, string destination, Money price)
    {
        Id = id;
        Destination = destination;
        Price = price;
    }
}

public readonly struct Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}

public record CreateBookingRequest(
    string Destination,
    decimal Amount,
    string Currency
);