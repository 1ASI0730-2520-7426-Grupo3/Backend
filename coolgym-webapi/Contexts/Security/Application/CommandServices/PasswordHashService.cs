using coolgym_webapi.Contexts.Security.Domain.Services;

namespace coolgym_webapi.Contexts.Security.Application.CommandServices;

/// <summary>
/// BCrypt-based password hashing implementation 
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    private const int WorkFactor = 11; // BCrypt work factor 

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // BCrypt.Net automatically generates salt
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false; // Invalid hash format
        }
    }
}
