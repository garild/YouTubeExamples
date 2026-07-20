 using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelBooking.Application.Bookings.Commands;
using HotelBooking.Application.Bookings.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HotelBooking.IntegrationTests.Bookings;

public class BookRoomTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BookRoomTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BookRoom_ShouldCreateBookingAndReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var roomId = Guid.Parse("d128df67-5d7d-417d-8959-1e30a597a731"); // Pre-seeded Room ID
        var command = new BookRoomCommand(
            roomId,
            "Jane Doe",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/bookings", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var bookingDto = await response.Content.ReadFromJsonAsync<BookingDto>();
        bookingDto.Should().NotBeNull();
        bookingDto!.RoomId.Should().Be(roomId);
        bookingDto.GuestName.Should().Be("Jane Doe");
        bookingDto.TotalPrice.Should().Be(300.00m); // 2 nights * $150
    }

    [Fact]
    public async Task BookRoom_ShouldReturnConflict_WhenBookingDatesOverlap()
    {
        // Arrange
        var roomId = Guid.Parse("d128df67-5d7d-417d-8959-1e30a597a731");
        
        var command1 = new BookRoomCommand(
            roomId,
            "Alice Smith",
            DateTime.UtcNow.AddDays(5),
            DateTime.UtcNow.AddDays(10)
        );

        var command2 = new BookRoomCommand(
            roomId,
            "Bob Jones",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(12)
        );

        // Act
        // Book the room once
        var response1 = await _client.PostAsJsonAsync("/api/bookings", command1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Attempt to book overlapping dates
        var response2 = await _client.PostAsJsonAsync("/api/bookings", command2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
