using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;

namespace coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;

/// <summary>
/// Password value object with basic validation (simplified)
/// </summary>
public record Password
{
    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw PasswordValidationException.EmptyPassword();

        if (value.Length < SecurityDomainConstants.MinPasswordLength)
            throw PasswordValidationException.TooShort(value.Length);

        if (value.Length > SecurityDomainConstants.MaxPasswordLength)
            throw PasswordValidationException.TooLong(value.Length);

        Value = value;
    }

    public string Value { get; init; }
}
