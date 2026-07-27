using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaylaAI.Booking.Application.Interfaces;
using WaylaAI.Booking.Infrastructure.Database;
using WaylaAI.Booking.Infrastructure.Repositories;
using WaylaAI.Booking.Infrastructure.Services;
using Refit;
namespace WaylaAI.Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options => options.UseInMemoryDatabase("BookingDb")); // Use InMemoryDatabase for testing purposes. Replace with actual database provider in production.

        services.AddTransient<Auth0TokenHandler>();
        services.AddScoped<IAuth0TokenProvider, Auth0TokenProvider>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        //services.AddHttpContextAccessor();
        //services.AddHttpClient<IPaymentService, WaylaAI.Booking.Infrastructure.Services.PaymentService>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration["PaymentApiBaseUrl"] ?? "https://localhost:7197");
        //}).AddHttpMessageHandler<Auth0TokenHandler>()
        //    .AddHeaderPropagation();

        return services;
    }
}
