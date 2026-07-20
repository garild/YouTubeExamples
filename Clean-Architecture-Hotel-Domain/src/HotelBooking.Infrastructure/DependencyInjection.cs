using HotelBooking.Application.Common.Interfaces;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("HotelBookingDb"));

        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
