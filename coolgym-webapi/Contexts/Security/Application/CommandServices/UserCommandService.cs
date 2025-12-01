using coolgym_webapi.Contexts.Security.Domain.Commands;
using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Security.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Security.Application.CommandServices;

/// <summary>
///     Authentication command service - handles registration and login
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    IPasswordHashService passwordHasher,
    IJwtTokenService tokenService,
    IUnitOfWork unitOfWork) : IUserCommandService
{
    /// <summary>
    ///     Register new user
    /// </summary>
    public async Task<User> Handle(CreateUserCommand command)
    {
        // Check for duplicate email
        var existingUserByEmail = await userRepository.FindByEmailAsync(command.Email);
        if (existingUserByEmail != null)
            throw RegistrationException.EmailAlreadyExists(command.Email);

        // Check for duplicate username
        var existingUserByUsername = await userRepository.FindByUsernameAsync(command.Username);
        if (existingUserByUsername != null)
            throw RegistrationException.UsernameAlreadyExists(command.Username);

        // Validate password 
        var password = new Password(command.Password);

        // Hash password
        var passwordHash = passwordHasher.HashPassword(password.Value);

        // Validate and create email value object
        var email = new Email(command.Email);

        // Parse role
        var role = UserRoleExtensions.FromString(command.Role);

        // Create user entity
        var user = new User(
            command.Username,
            email,
            passwordHash,
            command.Name,
            command.Phone,
            command.Type,
            role
        );

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();

        return user;
    }

    /// <summary>
    ///     Update user profile
    /// </summary>
    public async Task<User?> Handle(UpdateUserProfileCommand command)
    {
        var user = await userRepository.FindByIdAsync(command.UserId);
        if (user == null) return null;

        // Update profile fields if provided
        if (!string.IsNullOrEmpty(command.Name) || command.Phone != null || command.ProfilePhoto != null)
            user.UpdateProfile(
                command.Name ?? user.Name,
                command.Phone,
                command.ProfilePhoto
            );

        // Update client plan if provided (no validation on planId for now)
        // In production, you'd want to validate the planId exists
        if (command.ClientPlanId.HasValue)
            // Since we don't have a setter, we need to use reflection or add a method to User entity
            // For now, let's add an UpdateClientPlan method to User entity
            user.UpdateClientPlan(command.ClientPlanId.Value);

        userRepository.Update(user);
        await unitOfWork.CompleteAsync();

        return user;
    }

    /// <summary>
    ///     Authenticate user and generate JWT token
    /// </summary>
    public async Task<(User User, string AccessToken)> Handle(LoginUserCommand command)
    {
        // Find user by email
        var user = await userRepository.FindByEmailAsync(command.Email);

        // Check if user exists and is not deleted
        if (user == null || user.IsDeleted == SecurityDomainConstants.DeletedFlagTrue)
        {
            // Perform dummy hash to prevent timing attacks
            passwordHasher.HashPassword("dummy_password");
            throw AuthenticationException.InvalidCredentials();
        }

        // Verify password
        if (!passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
            throw AuthenticationException.InvalidCredentials();

        // Generate JWT token
        var accessToken = tokenService.GenerateAccessToken(user);

        return (user, accessToken);
    }
}