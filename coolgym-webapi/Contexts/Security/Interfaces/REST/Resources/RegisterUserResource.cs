using System.ComponentModel.DataAnnotations;

namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

public record RegisterUserResource
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Phone]
    public string? Phone { get; init; }

    [Required]
    [RegularExpression("^(individual|company)$")]
    public string Type { get; init; } = "individual";

    [Required]
    [RegularExpression("^(Client|Provider)$")]
    public string Role { get; init; } = "Client";
}
