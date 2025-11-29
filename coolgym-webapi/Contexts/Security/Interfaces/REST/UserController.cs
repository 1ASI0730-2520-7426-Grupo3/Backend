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
/// Authentication controller - handles login and register (simplified)
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UserController(
    IUserCommandService authCommandService,
    IStringLocalizer<UserController> localizer) : ControllerBase
{
    /// <summary>
    /// Register new user account
    /// </summary>
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
    /// Login with email and password
    /// </summary>
    /// <remarks>
    /// Authenticates user and returns JWT access token.

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
    /// Get user by ID to test the valid tkoen
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
}
