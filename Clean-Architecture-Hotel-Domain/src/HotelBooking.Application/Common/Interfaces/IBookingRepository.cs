using HotelBooking.Domain.Entities;
using HotelBooking.Domain.ValueObjects;

namespace HotelBooking.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid roomId, BookingRange range, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
