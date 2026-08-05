using Microsoft.EntityFrameworkCore;
using WaylaAI.Booking.Application.Interfaces;
using WaylaAI.Booking.Infrastructure.Database;

namespace WaylaAI.Booking.Infrastructure.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<WaylaAI.Booking.Domain.Entities.Booking>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(WaylaAI.Booking.Domain.Entities.Booking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
