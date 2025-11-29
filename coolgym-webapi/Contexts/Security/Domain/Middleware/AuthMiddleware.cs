using coolgym_webapi.Contexts.Security.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace coolgym_webapi.Contexts.Security.Domain.Middleware;

/// <summary>
/// Custom authentication middleware for JWT token validation
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
        // Skip authentication for endpoints with [AllowAnonymous]
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

        await _next(context);
    }
}
