using FluentAssertions;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.ValueObjects;
using Xunit;

namespace HotelBooking.UnitTests.Domain;

public class BookingTests
{
    [Fact]
    public void Constructor_ShouldCalculateCorrectTotalPrice()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var guestName = "John Doe";
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(4); // 3 nights
        var range = new BookingRange(startDate, endDate);
        decimal pricePerNight = 100.00m;

        // Act
        var booking = new Booking(Guid.NewGuid(), roomId, guestName, range, pricePerNight);

        // Assert
        booking.TotalPrice.Should().Be(300.00m);
    }

    [Fact]
    public void BookingRange_ShouldThrowException_WhenStartDateIsInThePast()
    {
        // Arrange
        var pastStart = DateTime.UtcNow.AddDays(-1);
        var end = DateTime.UtcNow.AddDays(1);

        // Act
        var act = () => new BookingRange(pastStart, end);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Booking start date cannot be in the past.");
    }

    [Fact]
    public void BookingRange_ShouldThrowException_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(2);
        var invalidEnd = DateTime.UtcNow.AddDays(1);

        // Act
        var act = () => new BookingRange(start, invalidEnd);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Booking end date must be after start date.");
    }

    [Fact]
    public void BookingRange_Overlaps_ShouldReturnTrue_WhenRangesOverlap()
    {
        // Arrange
        var baseStart = DateTime.UtcNow.AddDays(2);
        var baseEnd = DateTime.UtcNow.AddDays(5);
        var baseRange = new BookingRange(baseStart, baseEnd);

        var overlappingRange = new BookingRange(
            DateTime.UtcNow.AddDays(4),
            DateTime.UtcNow.AddDays(7)
        );

        // Act
        var overlaps = baseRange.Overlaps(overlappingRange);

        // Assert
        overlaps.Should().BeTrue();
    }
}
