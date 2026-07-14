public interface ITenantContextAccessor
{
    public TenantContext? CurrentTenant { get; set; }
    public void Clear();
}

public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantContext?> _currentTenant = new();
    public TenantContext? CurrentTenant
    {
        get => _currentTenant.Value;
        set => _currentTenant.Value = value;
    }
    public void Clear() => _currentTenant.Value = null;
}