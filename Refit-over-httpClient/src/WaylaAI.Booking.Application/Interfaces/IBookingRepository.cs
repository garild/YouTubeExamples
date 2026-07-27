using WaylaAI.Booking.Domain.Entities;

namespace WaylaAI.Booking.Application.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<WaylaAI.Booking.Domain.Entities.Booking>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(WaylaAI.Booking.Domain.Entities.Booking booking, CancellationToken cancellationToken = default);
}
