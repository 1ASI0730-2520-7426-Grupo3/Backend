using coolgym_webapi.Contexts.Security.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Security.Domain.Services;

/// <summary>
///     JWT token generation and validation service
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    ///     Generate JWT access token for user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    ///     Validate JWT token and return user if valid
    /// </summary>
    Task<User?> ValidateTokenAsync(string token);
}