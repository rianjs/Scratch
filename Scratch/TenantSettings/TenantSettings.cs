namespace TenantSettings;

public record TenantSettings
{
    public string Tenant { get; init; }
    public string UpdatedBy { get; init; }
    public DateTime Timestamp { get; init; }
    public List<TenantSetting> Settings { get; init; }
}

public record TenantSetting
{
    public string Id { get; init; }
    public string Description { get; init; }
    public string Comment { get; init; }
    public string Value { get; init; }
    public string ValueType { get; init; }
    public bool CanModify { get; init; }
    public string Type { get; init; }
    public DateTime Timestamp { get; init; }
}
