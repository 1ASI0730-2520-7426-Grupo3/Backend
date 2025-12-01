namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

public record AuthenticationResultResource
{
    public UserResource User { get; init; } = null!;
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
