namespace TenantSettings;

public record Environment
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public required string Jwt { get; init; }
    public required IReadOnlyCollection<string> Tenants { get; init; }

}