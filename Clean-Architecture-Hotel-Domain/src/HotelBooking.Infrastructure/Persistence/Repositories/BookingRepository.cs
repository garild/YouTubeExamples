using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(Guid roomId, BookingRange range, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .AnyAsync(b => b.RoomId == roomId &&
                           b.Range.StartDate < range.EndDate &&
                           range.StartDate < b.Range.EndDate,
                       cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
