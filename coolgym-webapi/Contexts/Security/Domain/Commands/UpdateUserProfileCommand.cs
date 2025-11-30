namespace coolgym_webapi.Contexts.Security.Domain.Commands;

/// <summary>
/// Command to update user profile (name, phone, photo, plan)
/// </summary>
public record UpdateUserProfileCommand(
    int UserId,
    string? Name,
    string? Phone,
    string? ProfilePhoto,
    int? ClientPlanId);
