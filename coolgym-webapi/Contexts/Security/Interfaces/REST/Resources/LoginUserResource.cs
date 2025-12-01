using System.ComponentModel.DataAnnotations;

namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

public record LoginUserResource
{
    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;
}