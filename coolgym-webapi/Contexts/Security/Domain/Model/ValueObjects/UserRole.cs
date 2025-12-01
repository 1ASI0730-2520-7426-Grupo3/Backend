namespace coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;

/// <summary>
///     User role enum - defines authorization roles
/// </summary>
public enum UserRole
{
    Client = 0,
    Provider = 1
}

public static class UserRoleExtensions
{
    public static string ToRoleName(this UserRole role)
    {
        return role switch
        {
            UserRole.Client => "Client",
            UserRole.Provider => "Provider",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }

    public static UserRole FromString(string roleName)
    {
        return roleName switch
        {
            "Client" => UserRole.Client,
            "Provider" => UserRole.Provider,
            _ => throw new ArgumentException($"Invalid role: {roleName}")
        };
    }
}