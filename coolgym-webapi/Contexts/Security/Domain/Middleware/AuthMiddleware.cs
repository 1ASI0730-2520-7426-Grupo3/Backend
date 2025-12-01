using System.Security.Claims;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Security.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace coolgym_webapi.Contexts.Security.Domain.Middleware;

/// <summary>
///     Custom authentication middleware for JWT token validation
/// </summary>
public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtTokenService jwtTokenService)
    {
        var isAnonymousEndpoint = context.GetEndpoint()?.Metadata
            .GetMetadata<AllowAnonymousAttribute>() != null;

        if (isAnonymousEndpoint)
        {
            await _next(context);
            return;
        }

        // Extract token from Authorization header
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (token is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: No token provided");
            return;
        }

        // Validate token
        var user = await jwtTokenService.ValidateTokenAsync(token);

        if (user is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid token");
            return;
        }

        // Store user in context for controllers
        context.Items["User"] = user;

        // Also set HttpContext.User for ASP.NET Core authorization middleware
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToRoleName())
        };
        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);
        context.User = principal;

        await _next(context);
    }
}