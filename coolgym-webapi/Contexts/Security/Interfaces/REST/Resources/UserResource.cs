namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

public record UserResource
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public int? ClientPlanId { get; init; }
    public string? ProfilePhoto { get; init; }
}