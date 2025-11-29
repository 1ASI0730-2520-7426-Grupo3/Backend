namespace coolgym_webapi.Contexts.Security.Domain.Model;

public static class SecurityDomainConstants
{
    // Password rules (simplified)
    public const int MinPasswordLength = 6;
    public const int MaxPasswordLength = 128;

    // Username rules
    public const int MinUsernameLength = 3;
    public const int MaxUsernameLength = 50;

    // User types
    public static readonly string[] ValidUserTypes = { "individual", "company" };

    // Soft delete
    public const int DeletedFlagTrue = 1;
    public const int DeletedFlagFalse = 0;
}
