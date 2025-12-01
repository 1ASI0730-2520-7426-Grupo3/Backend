namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

/// <summary>
///     Resource for updating user profile
/// </summary>
public record UpdateUserProfileResource
{
    public string? Name { get; init; }
    public string? Phone { get; init; }
    public string? ProfilePhoto { get; init; }
    public int? ClientPlanId { get; init; }
}