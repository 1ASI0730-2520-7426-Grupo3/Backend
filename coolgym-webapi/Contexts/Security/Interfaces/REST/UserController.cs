using System.Net.Mime;
using coolgym_webapi.Contexts.Security.Application.QueryServices;
using coolgym_webapi.Contexts.Security.Domain.Commands;
using coolgym_webapi.Contexts.Security.Domain.Model.Exceptions;
using coolgym_webapi.Contexts.Security.Domain.Services;
using coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.Security.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace coolgym_webapi.Contexts.Security.Interfaces.REST;

/// <summary>
///     Authentication controller - handles login and register
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UserController(
    IUserCommandService authCommandService,
    IStringLocalizer<UserController> localizer) : ControllerBase
{
    /// <summary>
    ///     Register new user account
    /// </summary>
    /// ///
    /// <summary>
    ///     Register new user account
    /// </summary>
    /// <remarks>
    ///     Creates a new user account with basic validation:
    ///     - Username: 3-50 characters, unique
    ///     - Email: Valid format, unique
    ///     - Password: Min 6 characters
    ///     - Role: "Client" or "Provider"
    ///     Example request:
    ///     POST /api/v1/auth/register
    ///     {
    ///     "username": "johndoe",
    ///     "email": "john@example.com",
    ///     "password": "password123",
    ///     "name": "John Doe",
    ///     "phone": "987654321",
    ///     "role": "Client"
    ///     }
    /// </remarks>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Validation failed</response>
    /// <response code="409">Email or username already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserResource resource)
    {
        try
        {
            var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource);
            var user = await authCommandService.Handle(command);
            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                userResource
            );
        }
        catch (PasswordValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UserValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (RegistrationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Login with email and password
    /// </summary>
    /// <remarks>
    ///     Authenticates user and returns JWT access token.
    ///     ///
    ///     <summary>
    ///         Login with email and password
    ///     </summary>
    ///     <remarks>
    ///         Authenticates user and returns JWT access token.
    ///         Example request:
    ///         POST /api/v1/auth/login
    ///         {
    ///         "email": "john@example.com",
    ///         "password": "password123"
    ///         }
    ///     </remarks>
    ///     <response code="200">Login successful, returns token</response>
    ///     <response code="400">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResultResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserResource resource)
    {
        try
        {
            var command = new LoginUserCommand(resource.Email, resource.Password);

            var (user, accessToken) = await authCommandService.Handle(command);

            var result = new AuthenticationResultResource
            {
                User = UserResourceFromEntityAssembler.ToResourceFromEntity(user),
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Ok(result);
        }
        catch (AuthenticationException)
        {
            return BadRequest(new { message = "Invalid email or password" });
        }
    }

    /// <summary>
    ///     Get user by ID to test the valid tkoen
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var queryService = HttpContext.RequestServices.GetRequiredService<IUserQueryService>();
        var user = await queryService.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });

        var resource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(resource);
    }

    /// <summary>
    ///     Update user profile (name, phone, photo, plan)
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileResource resource)
    {
        var command = new UpdateUserProfileCommand(
            id,
            resource.Name,
            resource.Phone,
            resource.ProfilePhoto,
            resource.ClientPlanId
        );

        var user = await authCommandService.Handle(command);
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });

        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(userResource);
    }
}