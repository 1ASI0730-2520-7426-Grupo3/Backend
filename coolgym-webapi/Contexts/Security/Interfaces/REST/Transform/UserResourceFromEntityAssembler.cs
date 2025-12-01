using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User entity)
    {
        return new UserResource
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email.Value,
            Name = entity.Name,
            Phone = entity.Phone,
            Type = entity.Type,
            Role = entity.Role.ToRoleName(),
            ClientPlanId = entity.ClientPlanId,
            ProfilePhoto = entity.ProfilePhoto
        };
    }
}
