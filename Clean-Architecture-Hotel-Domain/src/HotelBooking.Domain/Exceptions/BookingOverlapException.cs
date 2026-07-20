namespace HotelBooking.Domain.Exceptions;

public class BookingOverlapException : Exception
{
    public BookingOverlapException(Guid roomId, DateTime start, DateTime end)
        : base($"Room {roomId} is already booked from {start:yyyy-MM-dd} to {end:yyyy-MM-dd}.")
    {
    }
}
