namespace HotelBooking.Domain.ValueObjects;

public record BookingRange
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    private BookingRange() { } // Required for EF Core

    public BookingRange(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Booking start date cannot be in the past.");
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("Booking end date must be after start date.");
        }

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public int DurationInDays => (EndDate - StartDate).Days;

    public bool Overlaps(BookingRange other)
    {
        return StartDate < other.EndDate && other.StartDate < EndDate;
    }
}
