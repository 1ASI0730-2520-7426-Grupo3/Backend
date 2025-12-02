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
///     Authentication controller - handles login and register.
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UserController(
    IUserCommandService authCommandService,
    IStringLocalizer<UserController> localizer) : ControllerBase
{
    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <remarks>
    ///     Creates a new user account applying the following rules:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Username: 3–50 characters, unique.</description>
    ///         </item>
    ///         <item>
    ///             <description>Email: valid format, unique.</description>
    ///         </item>
    ///         <item>
    ///             <description>Password: minimum 6 characters.</description>
    ///         </item>
    ///         <item>
    ///             <description>Role: <c>"Client"</c> or <c>"Provider"</c>.</description>
    ///         </item>
    ///     </list>
    ///     <para>Sample request:</para>
    ///     <code language="json">
    ///     POST /api/v1/users/register
    ///     {
    ///       "username": "johndoe",
    ///       "email": "john@example.com",
    ///       "password": "password123",
    ///       "name": "John Doe",
    ///       "phone": "987654321",
    ///       "role": "Client"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation failed (invalid password or invalid user data).</response>
    /// <response code="409">Email or username already exists.</response>
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
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (UserValidationException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (RegistrationException ex)
        {
            return Conflict(new { message = localizer[ex.Message].Value });
        }
    }

    /// <summary>
    ///     Logs in a user with email and password.
    /// </summary>
    /// <remarks>
    ///     Authenticates the user and returns a JWT access token on success.
    ///     <para>Sample request:</para>
    ///     <code language="json">
    ///     POST /api/v1/users/login
    ///     {
    ///       "email": "john@example.com",
    ///       "password": "password123"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="200">Login successful, returns access token and user information.</response>
    /// <response code="400">Invalid email or password.</response>
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
            return BadRequest(new { message = localizer["Invalid email or password"].Value });
        }
    }

    /// <summary>
    ///     Gets a user by ID (used to test a valid token).
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var queryService = HttpContext.RequestServices.GetRequiredService<IUserQueryService>();
        var user = await queryService.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = localizer["User with ID {0} not found", id].Value });

        var resource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(resource);
    }

    /// <summary>
    ///     Updates user profile (name, phone, photo, plan).
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
            return NotFound(new { message = localizer["User with ID {0} not found", id].Value });

        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(userResource);
    }
}