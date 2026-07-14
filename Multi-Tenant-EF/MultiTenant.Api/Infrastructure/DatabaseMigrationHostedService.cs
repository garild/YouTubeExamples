using Microsoft.EntityFrameworkCore;


namespace MultiTenant.Api.Infrastructure
{
    public sealed class DatabaseMigrationHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IConfiguration _config;
        private readonly ILogger<DatabaseMigrationHostedService> _logger;
        public DatabaseMigrationHostedService(
            IServiceScopeFactory serviceScope,
            IConfiguration config,
            ILogger<DatabaseMigrationHostedService> logger)
        {
            _serviceScope = serviceScope;
            _config = config;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.GetValue("ApplyMigrationsOnStartup", false))
            {
                _logger.LogInformation("Startup migrations disabled.");
                return;
            }

            var tenantsSection = _config.GetSection("Tenants");

            if (tenantsSection is null)
                return;

            _logger.LogInformation("Applying EF Core migrations...");

            await using (var scope = _serviceScope.CreateAsyncScope())
            {
                foreach (var tenant in tenantsSection.GetChildren())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ITenantContextAccessor>();
                    var factory = scope.ServiceProvider.GetRequiredService<MultiTenantDbContext>();

                    context.CurrentTenant = new TenantContext(tenant.Key);

                    if (factory is null)
                        return;

                    factory.Database.SetConnectionString(tenant.Value);

                    await factory.Database.EnsureCreatedAsync();
                    await factory.Database.MigrateAsync(stoppingToken);
                    _logger.LogInformation("Migrations applied successfully.");
                }
            }
        }
    }
}