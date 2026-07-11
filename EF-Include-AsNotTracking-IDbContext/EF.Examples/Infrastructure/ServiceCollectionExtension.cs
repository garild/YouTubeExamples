using EF.Examples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EF.Examples.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseName = configuration.GetValue("InMemoryDatabase:Name", "EF.Examples.InMemory");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseLazyLoadingProxies()
                .UseInMemoryDatabase(databaseName));

        return services;
    }
}
