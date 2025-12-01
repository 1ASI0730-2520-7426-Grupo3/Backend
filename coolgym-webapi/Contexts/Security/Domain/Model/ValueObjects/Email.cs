using System.Text.RegularExpressions;
using coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;

namespace coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;

/// <summary>
///     Email value object with validation
/// </summary>
public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw UserValidationException.EmptyEmail();

        var trimmed = value.Trim().ToLowerInvariant();

        if (trimmed.Length > 255)
            throw UserValidationException.EmailTooLong(trimmed.Length);

        if (!EmailRegex.IsMatch(trimmed))
            throw UserValidationException.InvalidEmailFormat(trimmed);

        Value = trimmed;
    }

    public string Value { get; init; }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}