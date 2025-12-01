using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using coolgym_webapi.Contexts.Security.Application.QueryServices;
using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Security.Domain.Services;
using Microsoft.IdentityModel.Tokens;

namespace coolgym_webapi.Contexts.Security.Application.CommandServices;

/// <summary>
///     JWT token generation and validation service
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _secret;
    private readonly IServiceProvider _serviceProvider;

    public JwtTokenService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _secret = configuration["Jwt:Secret"]
                  ?? throw new InvalidOperationException("JWT secret not configured");
        _issuer = configuration["Jwt:Issuer"] ?? "CoolGym";
        _audience = configuration["Jwt:Audience"] ?? "CoolGymApp";
        _serviceProvider = serviceProvider;

        // Validate secret key length (minimum 32 characters)
        if (_secret.Length < 32)
            throw new InvalidOperationException("JWT secret must be at least 32 characters");
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim("username", user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToRoleName()),
            new Claim("user_type", user.Type),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1), // 1 hour expiration
            credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Extract user ID from claims
            // Note: ASP.NET Core transforms "sub" to NameIdentifier during JWT deserialization
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return null;

            // Create a scope to resolve scoped services properly
            using var scope = _serviceProvider.CreateScope();
            var userQueryService = scope.ServiceProvider.GetRequiredService<IUserQueryService>();

            // Fetch user from database
            var user = await userQueryService.GetByIdAsync(userId);

            // Check if user is deleted
            if (user == null || user.IsDeleted == SecurityDomainConstants.DeletedFlagTrue)
                return null;

            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }
}