using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Security.Domain.Model.Entities;

/// <summary>
/// User aggregate root - represents an authenticated user in the system (simplified)
/// </summary>
public class User : BaseEntity
{
    protected User()
    {
        // EF Core constructor
        Username = string.Empty;
        Email = null!;
        PasswordHash = string.Empty;
        Name = string.Empty;
        Type = "individual";
        Role = UserRole.Client;
    }

    public User(
        string username,
        Email email,
        string passwordHash,
        string name,
        string? phone,
        string type,
        UserRole role,
        int? clientPlanId = null,
        string? profilePhoto = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw UserValidationException.InvalidUsername();

        if (username.Length < SecurityDomainConstants.MinUsernameLength ||
            username.Length > SecurityDomainConstants.MaxUsernameLength)
            throw UserValidationException.UsernameLengthInvalid(username.Length);

        if (string.IsNullOrWhiteSpace(name))
            throw UserValidationException.EmptyName();

        if (!SecurityDomainConstants.ValidUserTypes.Contains(type))
            throw UserValidationException.InvalidUserType(type);

        Username = username.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Name = name.Trim();
        Phone = phone?.Trim();
        Type = type;
        Role = role;
        ClientPlanId = clientPlanId;
        ProfilePhoto = profilePhoto;
        CreatedDate = DateTime.UtcNow;
    }

    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Name { get; private set; }
    public string? Phone { get; private set; }
    public string Type { get; private set; } // 'individual' or 'company'
    public UserRole Role { get; private set; } // Client or Provider
    public int? ClientPlanId { get; private set; }
    public string? ProfilePhoto { get; private set; }

    // Business methods
    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty");

        PasswordHash = newPasswordHash;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateProfile(string name, string? phone, string? profilePhoto)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw UserValidationException.EmptyName();

        Name = name.Trim();
        Phone = phone?.Trim();
        ProfilePhoto = profilePhoto;
        UpdatedDate = DateTime.UtcNow;
    }

    public bool IsClient() => Role == UserRole.Client;
    public bool IsProvider() => Role == UserRole.Provider;
}
