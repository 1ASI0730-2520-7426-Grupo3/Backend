using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Rentals.Interfaces.REST.Transform;

public static class RentalRequestResourceFromEntityAssembler
{
    public static RentalRequestResource ToResourceFromEntity(RentalRequest entity)
    {
        return new RentalRequestResource(
            entity.Id,
            entity.EquipmentId,
            entity.Equipment?.Name,
            entity.Equipment?.Type,
            entity.Equipment?.Image,
            entity.ClientId,
            entity.Client?.Email?.Value,
            entity.ProviderId,
            entity.Provider?.Email?.Value,
            entity.Provider?.Name,
            entity.RequestDate,
            entity.Status,
            entity.Notes,
            entity.MonthlyPrice,
            entity.CreatedDate,
            entity.UpdatedDate ?? entity.CreatedDate);
    }

    public static IEnumerable<RentalRequestResource> ToResourceFromEntity(IEnumerable<RentalRequest> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}