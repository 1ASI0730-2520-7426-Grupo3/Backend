using coolgym_webapi.Contexts.Security.Domain.Commands;
using coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Transform;

public static class CreateUserCommandFromResourceAssembler
{
    public static CreateUserCommand ToCommandFromResource(RegisterUserResource resource)
    {
        return new CreateUserCommand(
            resource.Username,
            resource.Email,
            resource.Password,
            resource.Name,
            resource.Phone ?? string.Empty, // Default to empty if not provided
            resource.Type ?? "individual", // Default to individual
            resource.Role ?? "Client" // Default to Client
        );
    }
}