namespace coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;

// User validation exceptions
public class UserValidationException : Exception
{
    private UserValidationException(string key) : base(key)
    {
    }

    public static UserValidationException InvalidUsername()
    {
        return new UserValidationException("UserInvalidUsername");
    }

    public static UserValidationException UsernameLengthInvalid(int length)
    {
        return new UserValidationException($"UserUsernameLengthInvalid:{length}");
    }

    public static UserValidationException EmptyName()
    {
        return new UserValidationException("UserEmptyName");
    }

    public static UserValidationException EmptyEmail()
    {
        return new UserValidationException("UserEmptyEmail");
    }

    public static UserValidationException EmailTooLong(int length)
    {
        return new UserValidationException($"UserEmailTooLong:{length}");
    }

    public static UserValidationException InvalidEmailFormat(string email)
    {
        return new UserValidationException($"UserInvalidEmailFormat:{email}");
    }

    public static UserValidationException InvalidUserType(string type)
    {
        return new UserValidationException($"UserInvalidType:{type}");
    }
}

// Password validation exceptions
public class PasswordValidationException : Exception
{
    private PasswordValidationException(string key) : base(key)
    {
    }

    public static PasswordValidationException EmptyPassword()
    {
        return new PasswordValidationException("PasswordEmpty");
    }

    public static PasswordValidationException TooShort(int length)
    {
        return new PasswordValidationException($"PasswordTooShort:{length}");
    }

    public static PasswordValidationException TooLong(int length)
    {
        return new PasswordValidationException($"PasswordTooLong:{length}");
    }
}

// Authentication exceptions
public class AuthenticationException : Exception
{
    public const string ResourceKey = "AuthenticationFailed";

    public AuthenticationException(string message) : base(message)
    {
    }

    public static AuthenticationException InvalidCredentials()
    {
        return new AuthenticationException("InvalidCredentials");
    }
}

// Registration exceptions
public class RegistrationException : Exception
{
    private RegistrationException(string key) : base(key)
    {
    }

    public static RegistrationException EmailAlreadyExists(string email)
    {
        return new RegistrationException($"EmailAlreadyExists:{email}");
    }

    public static RegistrationException UsernameAlreadyExists(string username)
    {
        return new RegistrationException($"UsernameAlreadyExists:{username}");
    }
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