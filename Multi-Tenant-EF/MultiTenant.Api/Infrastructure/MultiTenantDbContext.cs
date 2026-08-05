using Microsoft.EntityFrameworkCore;
using MultiTenant.Api.Enitites;

namespace MultiTenant.Api.Infrastructure
{
    public sealed class MultiTenantDbContext : DbContext
    {
        private readonly ITenantContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;

        public MultiTenantDbContext(ITenantContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured)
                return;

            var tenantId = _contextAccessor.CurrentTenant?.TenantId;

            // Db Migration case
            if(tenantId is null)
            {
                optionsBuilder.UseNpgsql();
                return;
            }

            var connectionString = TenantResolver.ResolveConnectionString(tenantId, _configuration);

            if(string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connection string for tenant '{tenantId}' is not configured.");

            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure();
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MultiTenantDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }

}
