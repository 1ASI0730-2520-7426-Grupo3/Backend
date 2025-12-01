namespace coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;

// User validation exceptions
public class UserValidationException : Exception
{
    private UserValidationException(string key) : base(key) { }

    public static UserValidationException InvalidUsername()
        => new("UserInvalidUsername");

    public static UserValidationException UsernameLengthInvalid(int length)
        => new($"UserUsernameLengthInvalid:{length}");

    public static UserValidationException EmptyName()
        => new("UserEmptyName");

    public static UserValidationException EmptyEmail()
        => new("UserEmptyEmail");

    public static UserValidationException EmailTooLong(int length)
        => new($"UserEmailTooLong:{length}");

    public static UserValidationException InvalidEmailFormat(string email)
        => new($"UserInvalidEmailFormat:{email}");

    public static UserValidationException InvalidUserType(string type)
        => new($"UserInvalidType:{type}");
}

// Password validation exceptions
public class PasswordValidationException : Exception
{
    private PasswordValidationException(string key) : base(key) { }

    public static PasswordValidationException EmptyPassword()
        => new("PasswordEmpty");

    public static PasswordValidationException TooShort(int length)
        => new($"PasswordTooShort:{length}");

    public static PasswordValidationException TooLong(int length)
        => new($"PasswordTooLong:{length}");
}

// Authentication exceptions
public class AuthenticationException : Exception
{
    public const string ResourceKey = "AuthenticationFailed";

    public AuthenticationException(string message) : base(message) { }

    public static AuthenticationException InvalidCredentials()
        => new("InvalidCredentials");
}

// Registration exceptions
public class RegistrationException : Exception
{
    private RegistrationException(string key) : base(key) { }

    public static RegistrationException EmailAlreadyExists(string email)
        => new($"EmailAlreadyExists:{email}");

    public static RegistrationException UsernameAlreadyExists(string username)
        => new($"UsernameAlreadyExists:{username}");
}

// User not found exception
public class UserNotFoundException : Exception
{
    public const string ResourceKey = "UserNotFound";

    public UserNotFoundException(int id)
    {
        UserId = id;
    }

    public int UserId { get; }
}
