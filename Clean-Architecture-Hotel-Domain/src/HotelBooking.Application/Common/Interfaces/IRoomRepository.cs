using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Common.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
}
