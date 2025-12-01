namespace coolgym_webapi.Contexts.Security.Domain.Commands;

/// <summary>
/// Command to register a new user
/// </summary>
public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    string Name,
    string? Phone,
    string Type,
    string Role);
