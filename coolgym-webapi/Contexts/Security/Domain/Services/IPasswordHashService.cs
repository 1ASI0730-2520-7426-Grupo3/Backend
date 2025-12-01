namespace coolgym_webapi.Contexts.Security.Domain.Services;

/// <summary>
/// Password hashing service interface
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Hash a plain text password using BCrypt
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify password against hash
    /// </summary>
    bool VerifyPassword(string password, string hash);
}
