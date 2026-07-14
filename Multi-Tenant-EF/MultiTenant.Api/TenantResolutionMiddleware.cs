namespace MultiTenant.Api
{
    public sealed class TenantResolutionMiddleware : IMiddleware
    {
        private readonly ITenantContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private const string TenantHeaderName = "X-Tenant-ID";

        public TenantResolutionMiddleware(ITenantContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    await next(context);
                    return;
                }

                var tenantId = context.Request.Headers[TenantHeaderName].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    var tenantContext = new TenantContext(tenantId);

                    TenantResolver.ResolveConnectionString(tenantId, _configuration);

                    _contextAccessor.CurrentTenant = tenantContext;
                }

                await next(context);
            }
            catch (Exception)
            {
                _contextAccessor.Clear();
                throw;
            }
        }
    }
}
