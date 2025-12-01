namespace coolgym_webapi.Contexts.Security.Domain.Commands;

/// <summary>
///     Command to authenticate user
/// </summary>
public record LoginUserCommand(
    string Email,
    string Password);