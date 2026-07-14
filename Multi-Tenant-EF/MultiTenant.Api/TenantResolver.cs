namespace MultiTenant.Api
{
    public static class TenantResolver
    {
        public static string? ResolveConnectionString(string? tenantId, IConfiguration configuration)
        {
            ArgumentException.ThrowIfNullOrEmpty(tenantId, nameof(tenantId));

            var tenantSection = configuration.GetSection($"Tenants:{tenantId.Trim().ToLowerInvariant()}");

            if(tenantSection is null)
                throw new InvalidOperationException($"Tenant '{tenantId}' not found in configuration.");


            var connectionString = tenantSection.Value;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Connection string for tenant '{tenantId}' not found.");
            }

            return connectionString;
        }
    }
}
