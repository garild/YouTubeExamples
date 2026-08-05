using HotelBooking.Application.Bookings.Common;
using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Exceptions;
using HotelBooking.Domain.ValueObjects;
using MediatR;

namespace HotelBooking.Application.Bookings.Commands;

public class BookRoomCommandHandler : IRequestHandler<BookRoomCommand, BookingDto>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookRoomCommandHandler(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingDto> Handle(BookRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room == null)
        {
            throw new KeyNotFoundException($"Room with ID {request.RoomId} not found.");
        }

        var range = new BookingRange(request.StartDate, request.EndDate);

        bool hasOverlap = await _bookingRepository.HasOverlapAsync(request.RoomId, range, cancellationToken);
        if (hasOverlap)
        {
            throw new BookingOverlapException(request.RoomId, range.StartDate, range.EndDate);
        }

        var booking = new Booking(
            Guid.NewGuid(),
            room.Id,
            request.GuestName,
            range,
            room.PricePerNight
        );

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return new BookingDto(
            booking.Id,
            booking.RoomId,
            booking.GuestName,
            booking.Range.StartDate,
            booking.Range.EndDate,
            booking.TotalPrice,
            booking.CreatedAt
        );
    }
}
