public interface ITenantContext 
{
    string TenantId { get; }
    bool IsResolved { get; }
}

public sealed class TenantContext : ITenantContext
{
    public string TenantId { get; private set; } = string.Empty;
    public bool IsResolved => !string.IsNullOrWhiteSpace(TenantId);

    public TenantContext(string? tenantId)
    {
        if(string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("TenantId cannot be null or empty.");

        TenantId = tenantId;
    }
}