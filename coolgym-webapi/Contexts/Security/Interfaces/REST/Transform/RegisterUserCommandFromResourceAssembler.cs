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
            resource.Phone,
            resource.Type,
            resource.Role
        );
    }
}
